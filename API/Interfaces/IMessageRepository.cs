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
		Task<bool> SaveAllAsync(); 
		Task<IEnumerable<ConversationDto>> GetConversations(string currentUsername);
	}
}