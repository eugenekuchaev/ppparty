using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class EventsController : ControllerBase
	{
		private readonly IPhotoService _photoService;
		private readonly IMapper _mapper;
		private readonly InputProcessor _inputProcessor;
		private readonly IUnitOfWork _unitOfWork;

		public EventsController(IUnitOfWork unitOfWork, IPhotoService photoService, IMapper mapper, InputProcessor inputProcessor)
		{
			_unitOfWork = unitOfWork;
			_inputProcessor = inputProcessor;
			_mapper = mapper;
			_photoService = photoService;
		}

		[HttpGet("all-events")]
		public async Task<ActionResult<IEnumerable<EventDto>>> GetAllEvents([FromQuery] EventParams eventParams)
		{
			var events = await _unitOfWork.EventRepository.GetAllEventsAsync(eventParams);

			Response.AddPaginationHeader(events.CurrentPage, events.PageSize, events.TotalCount, events.TotalPages);

			return Ok(events);
		}

		[HttpGet("owned-events")]
		public async Task<ActionResult<IEnumerable<EventDto>>> GetOwnedEvents()
		{
			return Ok(await _unitOfWork.EventRepository.GetOwnedEventsAsync(User.GetUsername()));
		}

		[HttpGet("participated-events")]
		public async Task<ActionResult<IEnumerable<EventDto>>> GetParticipatedEvents()
		{
			return Ok(await _unitOfWork.EventRepository.GetParticipatedEventsAsync(User.GetUsername()));
		}

		[HttpGet("friends-events")]
		public async Task<ActionResult<IEnumerable<EventDto>>> GetFriendsEvents()
		{
			return Ok(await _unitOfWork.EventRepository.GetFriendsEventsAsync(User.GetUsername()));
		}

		[HttpGet("recommended-events")]
		public async Task<ActionResult<IEnumerable<EventDto>>> GetRecommendedEvents()
		{
			return Ok(await _unitOfWork.EventRepository.GetRecommendedEventsAsync(User.GetUsername()));
		}

		[HttpGet("{eventId}", Name = "GetEvent")]
		public async Task<ActionResult<FullEventDto>> GetEvent(int eventId)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
			var appEvent = await _unitOfWork.EventRepository.GetEventAsync(eventId, user!.Id);

			if (appEvent != null)
			{
				return appEvent;
			}

			return NotFound("Event not found");
		}

		[HttpPost]
		public async Task<ActionResult<Event>> CreateEvent(EventDto eventDto)
		{
			if (_inputProcessor.HasInvalidDates(eventDto.EventDates!, out string? invalidDateMessage))
			{
				return BadRequest(invalidDateMessage);
			}

			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
			var appEvent = _mapper.Map<Event>(eventDto);

			if (!_inputProcessor.IsValidInputForInterestsAndTags(eventDto.EventTagsString!))
			{
				return BadRequest("You need to add tags");
			}

			appEvent.EventTags = new List<EventTag>();

			var processResult = await ProcessEventTags(appEvent, eventDto.EventTagsString!);

			if (!processResult.IsSuccess)
			{
				return BadRequest(processResult.ErrorMessage);
			}

			appEvent.EventPhoto = new EventPhoto
			{
				PhotoUrl = "https://res.cloudinary.com/duy1fjz1z/image/upload/v1681336855/bisnwfwnxyg6flumjed4.png"
			};
			appEvent.EventOwner = user!;
			appEvent.Participants = new List<AppUser>();
			appEvent.Participants.Add(user!);
			appEvent.Currency = "usd";

			_unitOfWork.EventRepository.AddEvent(appEvent);

			if (await _unitOfWork.Complete())
			{
				return CreatedAtRoute("GetEvent", new { eventId = appEvent.Id }, appEvent);
			}

			return BadRequest("Failed to create event");
		}

		[HttpPatch("participate-in-event/{eventId}")]
		public async Task<ActionResult> ParticipateInEvent(int eventId)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

			var appEvent = await _unitOfWork.EventRepository.GetEventAsync(eventId, user!.Id);

			if (appEvent == null)
			{
				return NotFound("There's no event with this Id");
			}

			if (appEvent.Participants!.Any(x => x.Username == user.UserName))
			{
				return BadRequest("You're already participating in this event");
			}

			await _unitOfWork.EventRepository.AddUserToEventParticipantsAsync(eventId, user!.UserName);

			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to participate in the event");
		}

		[HttpPatch("stop-participating-in-event/{eventId}")]
		public async Task<ActionResult> StopParticipatingInEvent(int eventId)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

			var appEvent = await _unitOfWork.EventRepository.GetEventAsync(eventId, user!.Id);

			if (appEvent == null)
			{
				return NotFound("There's no event with this Id");
			}

			if (appEvent.EventOwner!.Username == user.UserName)
			{
				return BadRequest("You can't stop participating in your own event");
			}

			await _unitOfWork.EventRepository.RemoveUserFromEventParticipantsAsync(eventId, user!.UserName);

			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to stop participating in the event");
		}

		[HttpPut("update-event/{eventId}")]
		public async Task<ActionResult> UpdateEvent(EventUpdateDto eventUpdateDto, int eventId)
		{
			if (_inputProcessor.HasInvalidDates(eventUpdateDto.EventDates!, out string? invalidDateMessage))
			{
				return BadRequest(invalidDateMessage);
			}

			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
			var appEvent = await _unitOfWork.EventRepository.GetEventEntityAsync(eventId);

			if (appEvent == null)
			{
				return NotFound("There's no event with this Id");
			}

			if (appEvent.EventOwnerId != user!.Id)
			{
				return BadRequest("You are not the owner of this event");
			}

			if (eventUpdateDto.EventDates == null)
			{
				return BadRequest("You have to add event dates");
			}
			
			bool areEventDatesEqual = appEvent.EventDates
				.Select(ed => (ed.StartDate, ed.EndDate))
				.SequenceEqual(eventUpdateDto.EventDates!
					.Select(ed => (ed.StartDate, ed.EndDate)));

			_mapper.Map(eventUpdateDto, appEvent);

			appEvent.EventDates.Clear();
			var newEventDates = _mapper.Map<IEnumerable<EventDate>>(eventUpdateDto.EventDates);
			appEvent.EventDates = newEventDates.ToList();

			appEvent.Currency = "usd";

			if (!areEventDatesEqual)
			{
				await NotifyEventParticipants(appEvent);
			}

			_unitOfWork.EventRepository.UpdateEvent(appEvent);

			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to update user");
		}

		[HttpPost("add-tags/{eventId}")]
		public async Task<ActionResult> AddTags(int eventId, [FromBody] string tags)
		{
			if (!_inputProcessor.IsValidInputForInterestsAndTags(tags))
			{
				return BadRequest("You need to add tags");
			}

			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
			var appEvent = await _unitOfWork.EventRepository.GetEventEntityAsync(eventId);

			if (appEvent == null)
			{
				return NotFound("There's no event with this Id");
			}

			if (appEvent.EventOwnerId != user!.Id)
			{
				return BadRequest("You are not the owner of this event");
			}

			var processResult = await ProcessEventTags(appEvent, tags);

			if (!processResult.IsSuccess)
			{
				return BadRequest(processResult.ErrorMessage);
			}

			if (await _unitOfWork.Complete())
			{
				return CreatedAtRoute("GetEvent", new { eventId = appEvent.Id }, null);
			}

			return BadRequest("Failed to add tags");
		}

		[HttpPatch("remove-tag/{eventId}")]
		public async Task<ActionResult> RemoveTag(int eventId, [FromBody]string tagName)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
			var appEvent = await _unitOfWork.EventRepository.GetEventEntityAsync(eventId);

			if (appEvent == null)
			{
				return NotFound("There's no event with this Id");
			}

			if (appEvent.EventOwnerId != user!.Id)
			{
				return BadRequest("You are not the owner of this event");
			}

			var eventTag = appEvent.EventTags.FirstOrDefault(x => x.EventTagName == tagName);

			if (eventTag == null)
			{
				return NotFound("There's no tag with this name");
			}
			
			if (!_inputProcessor.IsCorrectNumberOfElements(numberOfElementsInEntity: appEvent.EventTags.Count(), 
				out string? errorMessage, elementName: "tag", numberOfNewElements: -1))
			{
				return BadRequest(errorMessage);
			}

			appEvent.EventTags.Remove(eventTag);

			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to delete tag");
		}

		[HttpPost("add-photo/{eventId}")]
		public async Task<ActionResult<EventPhotoDto>> AddPhoto(IFormFile file, int eventId)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
			var appEvent = await _unitOfWork.EventRepository.GetEventEntityAsync(eventId);

			if (appEvent == null)
			{
				return NotFound("There's no event with this id");
			}

			if (appEvent.EventOwnerId != user!.Id)
			{
				return BadRequest("You are not the owner of this event");
			}

			var result = await _photoService.AddPhotoAsync(file, "event");

			if (result.Error != null)
			{
				return BadRequest(result.Error.Message);
			}

			var photo = new EventPhoto
			{
				PhotoUrl = result.SecureUrl.AbsoluteUri,
				PublicId = result.PublicId,
				Event = appEvent
			};

			_unitOfWork.EventRepository.AddPhoto(photo);

			if (await _unitOfWork.Complete())
			{
				return CreatedAtRoute("GetEvent", new { eventId = appEvent.Id }, _mapper.Map<EventPhotoDto>(photo));
			}

			return BadRequest("Problem adding photo");
		}

		[Authorize(Policy = "RequireModeratorRole")]
		[HttpDelete("delete-event/{eventId}")]
		public async Task<ActionResult> DeleteEvent(int eventId)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
			var appEvent = await _unitOfWork.EventRepository.GetEventEntityAsync(eventId);

			if (appEvent == null)
			{
				return NotFound("There's no event with this Id");
			}

			_unitOfWork.EventRepository.DeleteEvent(appEvent);

			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to delete event");
		}
		
		[HttpGet("invites")]
		public async Task<ActionResult<IEnumerable<EventDto>>> GetInvites()
		{
			return Ok(await _unitOfWork.EventRepository.GetInvitesAsync(User.GetUsername()));
		}

		[HttpPatch("invite-to-event/{eventId}")]
		public async Task<ActionResult> InviteToEvent(string username, int eventId)
		{
			var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
			var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

			if (recipient == null)
			{
				return NotFound("There's no such a user");
			}

			if (!await _unitOfWork.FriendsRepository.CheckIfUsersAreFriends(sender!.Id, recipient.Id))
			{
				return BadRequest("You are not friends with this user");
			}

			var appEvent = await _unitOfWork.EventRepository.GetEventEntityAsync(eventId);

			if (appEvent == null)
			{
				return NotFound("There's no event with this Id");
			}

			if (appEvent.Participants.Any(x => x.UserName == recipient.UserName))
			{
				return BadRequest("This user is already participating in this event");
			}

			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

			await _unitOfWork.EventRepository.AddUserToInvitedToEventAsync(eventId, user!.UserName);

			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to invite user to the event");
		}

		[HttpPatch("decline-invitation/{eventId}")]
		public async Task<ActionResult> DeclineInvitation(int eventId)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

			await _unitOfWork.EventRepository.RemoveUserFromEventInvitationsAsync(eventId, user!.UserName);

			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to decline invitation");
		}

		[HttpPatch("cancel-event/{eventId}")]
		public async Task<ActionResult> CancelEvent(int eventId)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

			var appEvent = await _unitOfWork.EventRepository.GetEventEntityAsync(eventId);

			if (appEvent == null)
			{
				return NotFound("There's no event with this Id");
			}

			if (appEvent.EventOwner!.UserName != user!.UserName)
			{
				return BadRequest("You are not the owner of this event");
			}

			if (appEvent.IsCancelled == true)
			{
				return BadRequest("This event has already been cancelled");
			}

			await _unitOfWork.EventRepository.CancelEvent(eventId);
			
			await NotifyEventParticipants(appEvent);

			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to cancel the event");
		}

		[HttpGet("event-notifications")]
		public async Task<ActionResult<IEnumerable<EventNotification>>> GetEventNotifications()
		{
			return Ok(await _unitOfWork.EventRepository.GetEventNotificationsAsync(User.GetUsername()));
		}

		[HttpPatch("read-event-notification/{notificationId}")]
		public async Task<ActionResult> ReadEventNotification(int notificationId)
		{
			await _unitOfWork.EventRepository.ReadEventNotification(notificationId);
			await _unitOfWork.Complete();
			
			return NoContent();
		}

		[HttpGet("number-of-owned-events/{username}")]
		public async Task<ActionResult<int>> GetNumberOfOwnedEvents(string username)
		{
			return await _unitOfWork.EventRepository.GetNumberOfOwnedEvents(username);
		}

		[HttpGet("events-for-invitations")]
		public async Task<ActionResult<IEnumerable<EventDto>>> GetEventsForInvitations(string friendUsername)
		{
			return Ok(await _unitOfWork.EventRepository.GetActualParticipatedAndOwnedEvents(User.GetUsername(), friendUsername));
		}

		[HttpGet("has-user-been-invited-to-event")]
		public ActionResult<bool> CheckIfUserHasBeenInvited(string username, int eventId)
		{
			return Ok(_unitOfWork.EventRepository.CheckIfUserHasBeenInvited(username, eventId));
		}

		// Helper methods
		private async Task<ProcessResult> ProcessEventTags(Event appEvent, string tags)
		{
			var eventTags = _inputProcessor.SplitString(tags);
			
			var processResult = new ProcessResult();
			
			if (!_inputProcessor.IsCorrectNumberOfElements(numberOfElementsInEntity: appEvent.EventTags.Count(), 
				out string? errorMessage, elementName: "tag", numberOfNewElements: eventTags.Count()))
			{
				processResult.IsSuccess = false;
				processResult.ErrorMessage = errorMessage;
				return processResult;
			}

			foreach (var eventTag in eventTags)
			{
				if (_inputProcessor.HasInvalidTags(appEvent, eventTag, out string? invalidTagMessage))
				{
					processResult.IsSuccess = false;
					processResult.ErrorMessage = invalidTagMessage;
					return processResult;
				}

				var eventTagFromDb = await _unitOfWork.EventRepository.GetEventTagByNameAsync(eventTag);

				if (eventTagFromDb != null)
				{
					appEvent.EventTags.Add(eventTagFromDb);
				}
				else
				{
					appEvent.EventTags.Add(new EventTag { EventTagName = eventTag });
				}
			}

			processResult.IsSuccess = true;
			return processResult;
		}

		private async Task NotifyEventParticipants(Event appEvent)
		{
			foreach (var participant in appEvent.Participants!)
			{
				var notification = new EventNotification
				{
					NotificationMessage = $"{appEvent.EventName} has been postponed or cancelled.",
					EventId = appEvent.Id,
					EventName = appEvent.EventName!
				};

				await _unitOfWork.EventRepository.NotifyEventParticipant(participant.Id, notification);
			}
		}
	}
}