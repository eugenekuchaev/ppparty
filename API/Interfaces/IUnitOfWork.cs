namespace API.Interfaces
{
	public interface IUnitOfWork
	{
		IUserRepository UserRepository { get; }
		IMessageRepository MessageRepository { get; }
		IFriendsRepository FriendsRepository { get; }
		IEventRepository EventRepository { get; }
		
		Task<bool> Complete();
		bool HasChanges();
	}
}