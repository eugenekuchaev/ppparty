namespace API.Entities
{
	public class AppUserFriend
	{
		public AppUser? AddingToFriendsUser { get; set; }
		public int AddingToFriendsUserId { get; set; }
		public AppUser? AddedToFriendsUser { get; set; }
		public int AddedToFriendsUserId { get; set; }
	}
}