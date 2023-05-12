using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
	public interface IMessageRepository
	{
		void AddMessage(Message message);
		void DeleteMessage(Message message);
		Task<Message?> GetMessage(int id);
		Task<PagedList<MessageDto>> GetMessageThread(UserParams userParams, string currentUsername, string recipientUsername);
		Task<IEnumerable<MessageDto>> GetMessageThreadWithoutParams(string currentUsername, string recipientUsername);
		Task<IEnumerable<ConversationDto>> GetConversations(string currentUsername);
		void AddGroup(Group group);
		void RemoveConnection(Connection connection);
		Task<Connection?> GetConnection(string connectionId);
		Task<Group?> GetMessageGroup(string groupName);
		Task<Group?> GetGroupForConnection(string connectionId);
	}
}