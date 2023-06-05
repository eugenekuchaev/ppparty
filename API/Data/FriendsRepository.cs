using API.DTOs;
using API.Entities;
using API.Enums;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	public class FriendsRepository : IFriendsRepository
	{
		private readonly DataContext _context;

		public FriendsRepository(DataContext context)
		{
			_context = context;
		}

		public async Task<AppUserFriend?> GetUserFriend(int addingToFriendsUserId, int addedToFriendsUserId)
		{
			return await _context.Friends.FindAsync(addingToFriendsUserId, addedToFriendsUserId);
		}

		public async Task<IEnumerable<FriendDto>> GetFriends(string predicate, int userId)
		{
			var users = _context.Users.AsQueryable();
			var friends = _context.Friends.AsQueryable();

			if (predicate == "friend-requests")
			{
				friends = friends.Where(friend => friend.AddedToFriendsUserId == userId);
				users = friends.Select(friend => friend.AddingToFriendsUser)!;
			}

			if (predicate == "added-to-friends")
			{
				friends = friends.Where(friend => friend.AddingToFriendsUserId == userId);
				users = friends.Select(friend => friend.AddedToFriendsUser)!;
			}

			if (predicate == "mutual-friends")
			{
				var friendsAddedTo = friends
					.Where(friend => friend.AddedToFriendsUserId == userId)
					.Select(friend => friend.AddingToFriendsUser);
				
				var friendsAddingTo = friends
					.Where(friend => friend.AddingToFriendsUserId == userId)
					.Select(friend => friend.AddedToFriendsUser);

				var intersection = await friendsAddedTo
					.Intersect(friendsAddingTo).ToListAsync();

				users = users.Where(user => intersection.Contains(user));
			}

			return await users.Select(user => new FriendDto
			{
				Id = user.Id,
				Username = user.UserName,
				FullName = user.FullName,
				UserPhotoUrl = user.UserPhoto.PhotoUrl,
				City = user.City
			}).ToListAsync();
		}

		public async Task<AppUser?> GetUserWithFriends(int userId)
		{
			return await _context.Users
				.Include(x => x.AddedToFriendsUsers)
				.FirstOrDefaultAsync(x => x.Id == userId);
		}
		
		public async Task<FriendshipStatus> CheckUsersFriendship(string currentUserUsername, string secondUserUsername)
		{
			var firstUserFriendship = await _context.Friends
				.FirstOrDefaultAsync(f => f.AddingToFriendsUser!.UserName == currentUserUsername && 
					f.AddedToFriendsUser!.UserName == secondUserUsername);

			var secondUserFrienship = await _context.Friends
				.FirstOrDefaultAsync(f => f.AddingToFriendsUser!.UserName == secondUserUsername && 
					f.AddedToFriendsUser!.UserName == currentUserUsername);

			if (firstUserFriendship == null && secondUserFrienship == null)
			{
				return FriendshipStatus.NoFriendship;
			}
			
			if (firstUserFriendship != null && secondUserFrienship == null)
			{
				return FriendshipStatus.AddingToFriends;
			}
			
			if (firstUserFriendship == null && secondUserFrienship != null)
			{
				return FriendshipStatus.AddedToFriends;
			}
			
			if (firstUserFriendship != null && secondUserFrienship != null)
			{
				return FriendshipStatus.AreFriends;
			}
			
			return FriendshipStatus.NoFriendship;
		}
	}
}