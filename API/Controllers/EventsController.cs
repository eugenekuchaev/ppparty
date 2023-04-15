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
		private readonly IEventRepository _eventRepository;
		private readonly IPhotoService _photoService;
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;
		private readonly InputProcessor _inputProcessor;
		private readonly IFriendsRepository _friendsRepository;

		public EventsController(IEventRepository eventRepository, IPhotoService photoService, IUserRepository userRepository,
			IMapper mapper, InputProcessor inputProcessor, IFriendsRepository friendsRepository)
		{
			_friendsRepository = friendsRepository;
			_inputProcessor = inputProcessor;
			_mapper = mapper;
			_userRepository = userRepository;
			_photoService = photoService;
			_eventRepository = eventRepository;
		}

		[HttpGet("allevents")]
		public async Task<ActionResult<IEnumerable<EventDto>>> GetAllEvents([FromQuery] EventParams eventParams)
		{
			var events = await _eventRepository.GetAllEventsAsync(eventParams);

			Response.AddPaginationHeader(events.CurrentPage, events.PageSize, events.TotalCount, events.TotalPages);

			return Ok(events);
		}

		[HttpGet("ownedevents")]
		public async Task<ActionResult<IEnumerable<EventDto>>> GetOwnedEvents()
		{
			return Ok(await _eventRepository.GetOwnedEventsAsync(User.GetUsername()));
		}

		[HttpGet("participatedevents")]
		public async Task<ActionResult<IEnumerable<EventDto>>> GetParticipatedEvents()
		{
			return Ok(await _eventRepository.GetParticipatedEventsAsync(User.GetUsername()));
		}

		[HttpGet("friendsevents")]
		public async Task<ActionResult<IEnumerable<EventDto>>> GetFriendsEvents()
		{
			return Ok(await _eventRepository.GetFriendsEventsAsync(User.GetUsername()));
		}

		[HttpGet("recommendedevents")]
		public async Task<ActionResult<IEnumerable<EventDto>>> GetRecommendedEvents()
		{
			return Ok(await _eventRepository.GetRecommendedEventsAsync(User.GetUsername()));
		}

		[HttpGet("{eventId}", Name = "GetEvent")]
		public async Task<ActionResult<FullEventDto>> GetEvent(int eventId)
		{
			var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
			var appEvent = await _eventRepository.GetEventAsync(eventId, user!.Id);

			if (appEvent != null)
			{
				return appEvent;
			}

			return BadRequest("Event not found");
		}

		[HttpPost]
		public async Task<ActionResult> CreateEvent(EventDto eventDto)
		{
			var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
			var appEvent = _mapper.Map<Event>(eventDto);
			string tags = eventDto.EventTagsString!;

			if (!_inputProcessor.ValidateInput(tags))
			{
				return BadRequest("You need to add tags");
			}

			appEvent.EventTags = new List<EventTag>();
			var eventTags = _inputProcessor.SplitString(tags);

			foreach (var eventTag in eventTags)
			{
				if (eventTag.Length > 32)
				{
					return BadRequest("One of the interests is too long");
				}

				if (appEvent.EventTags.Any(x => x.EventTagName == eventTag))
				{
					return BadRequest("There's already one of these tags in this event");
				}

				var eventTagFromDb = await _eventRepository.GetEventTagByNameAsync(eventTag);

				if (eventTagFromDb != null)
				{
					appEvent.EventTags.Add(eventTagFromDb);
				}
				else
				{
					appEvent.EventTags.Add(new EventTag { EventTagName = eventTag });
				}
			}

			appEvent.EventPhoto = new EventPhoto
			{
				PhotoUrl = "https://res.cloudinary.com/duy1fjz1z/image/upload/v1681336855/bisnwfwnxyg6flumjed4.png"
			};

			appEvent.EventOwner = user!;
			appEvent.Participants = new List<AppUser>();
			appEvent.Participants.Add(user!);
			appEvent.Currency = "usd";

			_eventRepository.AddEvent(appEvent);

			if (await _eventRepository.SaveAllAsync())
			{
				return Ok();
			}

			return BadRequest("Failed to create event");
		}

		[HttpPut("participateinevent/{eventId}")]
		public async Task<ActionResult> ParticipateInEvent(int eventId)
		{
			var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

			await _eventRepository.AddUserToEventParticipantsAsync(eventId, user!.UserName);

			if (await _eventRepository.SaveAllAsync())
			{
				return Ok();
			}

			return BadRequest("Failed to participate in the event");
		}
		
		[HttpDelete("stopparticipatinginevent/{eventId}")]
		public async Task<ActionResult> StopParticipatingInEvent(int eventId)
		{
			var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

			await _eventRepository.RemoveUserFromEventParticipantsAsync(eventId, user!.UserName);

			if (await _eventRepository.SaveAllAsync())
			{
				return Ok();
			}

			return BadRequest("Failed to participate in the event");
		}

		[HttpPut("updateevent/{eventId}")]
		public async Task<ActionResult> UpdateEvent(EventUpdateDto eventUpdateDto, int eventId)
		{
			var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
			var appEvent = await _eventRepository.GetEventEntityAsync(eventId);

			if (appEvent == null)
			{
				return BadRequest("There's no event with this Id");
			}
			
			if (appEvent.EventOwnerId != user!.Id)
			{
				return BadRequest("You are not the owner of this event");
			}

			_mapper.Map(eventUpdateDto, appEvent);

			_eventRepository.UpdateEvent(appEvent);

			if (await _eventRepository.SaveAllAsync())
			{
				return NoContent();
			}

			return BadRequest("Failed to update user");
		}

		[HttpPost("addtags/{eventId}")]
		public async Task<ActionResult> AddTags([FromBody] string tags, int eventId)
		{
			if (!_inputProcessor.ValidateInput(tags))
			{
				return BadRequest("You need to add tags");
			}

			var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
			var appEvent = await _eventRepository.GetEventEntityAsync(eventId);

			if (appEvent == null)
			{
				return BadRequest("There's no event with this Id");
			}
			
			if (appEvent.EventOwnerId != user!.Id)
			{
				return BadRequest("You are not the owner of this event");
			}

			var eventTags = _inputProcessor.SplitString(tags);

			foreach (var eventTag in eventTags)
			{
				if (eventTag.Length > 32)
				{
					return BadRequest("One of the interests is too long");
				}

				if (appEvent.EventTags.Any(x => x.EventTagName == eventTag))
				{
					return BadRequest("There's already one of these tags in this event");
				}

				var eventTagFromDb = await _eventRepository.GetEventTagByNameAsync(eventTag);

				if (eventTagFromDb != null)
				{
					appEvent.EventTags.Add(eventTagFromDb);
				}
				else
				{
					appEvent.EventTags.Add(new EventTag { EventTagName = eventTag });
				}
			}

			if (await _eventRepository.SaveAllAsync())
			{
				return NoContent();
			}

			return BadRequest("Failed to add tags");
		}

		[HttpDelete("deletetag/{eventId}")]
		public async Task<ActionResult> DeleteTag(string tagName, int eventId)
		{
			var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
			var appEvent = await _eventRepository.GetEventEntityAsync(eventId);

			if (appEvent == null)
			{
				return BadRequest("There's no event with this Id");
			}
			
			if (appEvent.EventOwnerId != user!.Id)
			{
				return BadRequest("You are not the owner of this event");
			}

			var eventTag = appEvent.EventTags.FirstOrDefault(x => x.EventTagName == tagName);

			if (eventTag == null)
			{
				return BadRequest("There's no tag with this name");
			}

			appEvent.EventTags.Remove(eventTag);

			if (await _eventRepository.SaveAllAsync())
			{
				return NoContent();
			}

			return BadRequest("Failed to delete tag");
		}

		[HttpPost("add-photo/{eventId}")]
		public async Task<ActionResult<EventPhotoDto>> AddPhoto(IFormFile file, int eventId)
		{
			var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
			var appEvent = await _eventRepository.GetEventEntityAsync(eventId);
			
			if (appEvent == null)
			{
				return BadRequest("There's no event with this id");
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

			_eventRepository.AddPhoto(photo);

			if (await _eventRepository.SaveAllAsync())
			{
				return CreatedAtRoute("GetEvent", new { eventId = appEvent.Id }, _mapper.Map<EventPhotoDto>(photo));
			}

			return BadRequest("Problem adding photo");
		}
		
		[HttpDelete("deleteevent/{eventId}")]
		public async Task<ActionResult> DeleteEvent(int eventId)
		{
			var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
			var appEvent = await _eventRepository.GetEventEntityAsync(eventId);
			
			if (appEvent == null)
			{
				return BadRequest("There's no event with this Id");
			}
			
			if (appEvent.EventOwnerId != user!.Id)
			{
				return BadRequest("You are not the owner of this event");
			}
			
			_eventRepository.DeleteEvent(appEvent);
			
			if (await _eventRepository.SaveAllAsync())
			{
				return NoContent();
			}

			return BadRequest("Failed to delete event");
		}
		
		[HttpPut("invitetoevent/{eventId}")]
		public async Task<ActionResult> InviteToEvent(string username, int eventId)
		{
			var sender = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
			var recipient = await _userRepository.GetUserByUsernameAsync(username);
			
			if (recipient == null)
			{
				return BadRequest("There's no such a user");
			}
			
			if (!await _friendsRepository.CheckIfUsersAreFriends(sender!.Id, recipient.Id))
			{
				return BadRequest("You are not friends with this user");
			}
			
			var appEvent = await _eventRepository.GetEventEntityAsync(eventId);
			
			var user = await _userRepository.GetUserByUsernameAsync(username);

			await _eventRepository.AddUserToInvitedToEventAsync(eventId, user!.UserName);

			if (await _eventRepository.SaveAllAsync())
			{
				return Ok();
			}

			return BadRequest("Failed to invite user to the event");
		}
		
		[HttpGet("invites")]
		public async Task<ActionResult<IEnumerable<EventDto>>> GetInvites()
		{
			return Ok(await _eventRepository.GetInvitesAsync(User.GetUsername()));
		}
		
		[HttpDelete("declineinvitation/{eventId}")]
		public async Task<ActionResult> DeclineInvitation(int eventId)
		{
			var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

			await _eventRepository.RemoveUserFromEventInvitationsAsync(eventId, user!.UserName);

			if (await _eventRepository.SaveAllAsync())
			{
				return Ok();
			}

			return BadRequest("Failed to decline invitation");
		}
	}
}