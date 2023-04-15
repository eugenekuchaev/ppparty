using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	public class EventRepository : IEventRepository
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;
		private readonly IUserRepository _userRepository;
		private readonly IFriendsRepository _friendsRepository;

		public EventRepository(DataContext context, IMapper mapper, IUserRepository userRepository,
			IFriendsRepository friendsRepository)
		{
			_friendsRepository = friendsRepository;
			_userRepository = userRepository;
			_mapper = mapper;
			_context = context;
		}

		public void AddEvent(Event appEvent)
		{
			_context.Events.Add(appEvent);
		}

		public async Task AddUserToEventParticipantsAsync(int eventId, string username)
		{
			var appEvent = await _context.Events
				.Include(x => x.Participants)
				.FirstOrDefaultAsync(x => x.Id == eventId);

			var user = await _userRepository.GetUserByUsernameAsync(username);

			if (appEvent != null && user != null)
			{
				appEvent.Participants.Add(user);
			}
		}
		
		public async Task RemoveUserFromEventParticipantsAsync(int eventId, string username)
		{
			var appEvent = await _context.Events
				.Include(x => x.Participants)
				.FirstOrDefaultAsync(x => x.Id == eventId);

			var user = await _userRepository.GetUserByUsernameAsync(username);

			if (appEvent != null && user != null)
			{
				appEvent.Participants.Remove(user);
			}
		}
		
		public async Task AddUserToInvitedToEventAsync(int eventId, string username)
		{
			var appEvent = await _context.Events
				.Include(x => x.InvitedToEvent)
				.FirstOrDefaultAsync(x => x.Id == eventId);

			var user = await _userRepository.GetUserByUsernameAsync(username);

			if (appEvent != null && user != null)
			{
				appEvent.InvitedToEvent!.Add(user);
			}
		}
		
		public async Task RemoveUserFromEventInvitationsAsync(int eventId, string username)
		{
			var appEvent = await _context.Events
				.Include(x => x.InvitedToEvent)
				.FirstOrDefaultAsync(x => x.Id == eventId);

			var user = await _userRepository.GetUserByUsernameAsync(username);

			if (appEvent != null && user != null)
			{
				appEvent.InvitedToEvent!.Remove(user);
			}
		}

		public void AddPhoto(EventPhoto photo)
		{
			_context.EventPhotos.Add(photo);
		}

		public void DeleteEvent(Event appEvent)
		{
			_context.Events.Remove(appEvent);
		}

		public async Task<PagedList<EventDto>> GetAllEventsAsync(EventParams eventParams)
		{
			var query = _context.Events.AsQueryable();

			if (!string.IsNullOrEmpty(eventParams.EventName))
			{
				query = query.Where(u => u.EventName.ToLower() == eventParams.EventName.ToLower());
			}

			if (eventParams.FromDate != null && eventParams.ToDate != null)
			{
				var fromDate = DateTime.Parse(eventParams.FromDate);
				var toDate = DateTime.Parse(eventParams.ToDate);

				query = query.Where(e => e.EventDates
					.Any(d => d.StartDate <= toDate
						&& d.EndDate >= fromDate));
			}

			if (!string.IsNullOrEmpty(eventParams.Country))
			{
				query = query.Where(u => u.Country != null && u.Country.ToLower() == eventParams.Country.ToLower());
			}

			if (!string.IsNullOrEmpty(eventParams.Region))
			{
				query = query.Where(u => u.Region != null && u.Region.ToLower() == eventParams.Region.ToLower());
			}

			if (!string.IsNullOrEmpty(eventParams.City))
			{
				query = query.Where(u => u.City != null && u.City.ToLower() == eventParams.City.ToLower());
			}

			if (!string.IsNullOrEmpty(eventParams.EventTag))
			{
				query = query.Where(u => u.EventTags!.Any(i =>
					i.EventTagName.ToLower() == eventParams.EventTag.ToLower()));
			}

			return await PagedList<EventDto>.CreateAsync(
				query.ProjectTo<EventDto>(_mapper.ConfigurationProvider).AsNoTracking(),
				eventParams.PageNumber,
				eventParams.PageSize);
		}

		public async Task<FullEventDto?> GetEventAsync(int eventId, int userId)
		{
			var eventDto = await _context.Events
				.Where(x => x.Id == eventId)
				.ProjectTo<FullEventDto>(_mapper.ConfigurationProvider)
				.SingleOrDefaultAsync();

			if (eventDto != null)
			{
				var eventParticipants = eventDto.Participants?.Select(x => x.Id) ?? new List<int>();
				var mutualFriends = await _friendsRepository.GetFriends("mutualfriends", userId);
				var friendsParticipantIds = mutualFriends
					.Where(friend => eventParticipants.Contains(friend.Id))
					.Select(friend => friend.Id)
					.ToList();

				if (friendsParticipantIds.Any())
				{
					var friendsParticipants = eventDto.Participants?
						.Where(x => friendsParticipantIds.Contains(x.Id))
						.ToList();

					if (friendsParticipants != null)
					{
						eventDto.FriendsParticipants = friendsParticipants;
					}
				}
			}

			return eventDto;
		}
		
		public async Task<Event?> GetEventEntityAsync(int eventId)
		{
			return await _context.Events
				.Where(x => x.Id == eventId)
				.Include(x => x.EventTags)
				.Include(x => x.EventPhoto)
				.SingleOrDefaultAsync();
		}

		public async Task<EventTag?> GetEventTagByNameAsync(string eventTagName)
		{
			return await _context.EventTags.SingleOrDefaultAsync(x => x.EventTagName == eventTagName);
		}

		public async Task<IEnumerable<EventDto>> GetOwnedEventsAsync(string username)
		{
			var events = await _context.Events
				.Where(x => x.EventOwner.UserName == username)
				.ProjectTo<EventDto>(_mapper.ConfigurationProvider)
				.ToListAsync();

			if (events != null)
			{
				return events;
			}

			return new List<EventDto>();
		}

		public async Task<IEnumerable<EventDto>> GetParticipatedEventsAsync(string username)
		{
			var events = await _context.Events
				.Where(x => x.Participants.Any(x => x.UserName == username))
				.ProjectTo<EventDto>(_mapper.ConfigurationProvider)
				.ToListAsync();

			if (events != null)
			{
				return events;
			}

			return new List<EventDto>();
		}

		public async Task<IEnumerable<EventDto>> GetFriendsEventsAsync(string username)
		{
			var user = await _userRepository.GetUserByUsernameAsync(username);

			var mutualFriends = await _friendsRepository.GetFriends("mutualfriends", user!.Id);

			var friendIds = mutualFriends.Select(f => f.Id).ToList();

			return await _context.Events
				.Where(e => e.Participants.Any(x => friendIds.Contains(x.Id)))
				.ProjectTo<EventDto>(_mapper.ConfigurationProvider)
				.ToListAsync();
		}

		public async Task<IEnumerable<EventDto>> GetRecommendedEventsAsync(string username)
		{
			var user = await _userRepository.GetUserByUsernameAsync(username);

			return await _context.Events
				.Where(e => e.City == user!.City && e.EventTags
					.Where(et => user.UserInterests!
						.Select(ui => ui.InterestName)
						.Contains(et.EventTagName))
						.Any())
				.ProjectTo<EventDto>(_mapper.ConfigurationProvider)
				.ToListAsync();
		}

		public async Task<bool> SaveAllAsync()
		{
			return await _context.SaveChangesAsync() > 0;
		}

		public void UpdateEvent(Event appEvent)
		{
			_context.Entry(appEvent).State = EntityState.Modified;
		}
		
		public async Task<IEnumerable<EventDto>> GetInvitesAsync(string username)
		{
			var events = await _context.Events
				.Where(x => x.InvitedToEvent!.Any(x => x.UserName == username))
				.ProjectTo<EventDto>(_mapper.ConfigurationProvider)
				.ToListAsync();

			if (events != null)
			{
				return events;
			}

			return new List<EventDto>();
		}
	}
}