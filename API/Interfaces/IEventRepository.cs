using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
	public interface IEventRepository
	{
		void AddEvent(Event appEvent);
		void DeleteEvent(Event appEvent);
		Task<PagedList<EventDto>> GetAllEventsAsync(EventParams eventParams);
		Task<IEnumerable<EventDto>> GetRecommendedEventsAsync(string username);
		Task<IEnumerable<EventDto>> GetOwnedEventsAsync(string username);
		Task<IEnumerable<EventDto>> GetParticipatedEventsAsync(string username);
		Task<IEnumerable<EventDto>> GetFriendsEventsAsync(string username);
		Task<FullEventDto?> GetEventAsync(int id, int userId);
		Task<Event?> GetEventEntityAsync(int eventId);
		Task<bool> SaveAllAsync();
		void AddPhoto(EventPhoto photo);
		void UpdateEvent(Event appEvent);
		Task<EventTag?> GetEventTagByNameAsync(string interestName);
		Task AddUserToEventParticipantsAsync(int eventId, string username);
		Task RemoveUserFromEventParticipantsAsync(int eventId, string username);
		Task AddUserToInvitedToEventAsync(int eventId, string username);
		Task<IEnumerable<EventDto>> GetInvitesAsync(string username);
		Task RemoveUserFromEventInvitationsAsync(int eventId, string username);
		Task CancelEvent(int eventId);
		Task NotifyEventParticipant(int userId, EventNotification notification);
		Task<IEnumerable<EventNotification>> GetEventNotificationsAsync(string username);
		Task ReadEventNotification(int notificationId);
		Task<int> GetNumberOfOwnedEvents(string username);
	}
}