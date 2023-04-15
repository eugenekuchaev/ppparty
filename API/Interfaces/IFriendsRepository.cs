using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
	public interface IFriendsRepository
	{
		Task<AppUserFriend?> GetUserFriend(int addingToFriendsUserId, int addedToFriendsUserId);
		Task<AppUser?> GetUserWithFriends(int userId);
		Task<IEnumerable<FriendDto>> GetFriends(string predicate, int userId);
		Task<bool> CheckIfUsersAreFriends(int firstUserId, int secondUserId);
	}
}