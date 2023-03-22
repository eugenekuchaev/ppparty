using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<AppUser> Users => Set<AppUser>();
		public DbSet<UserInterest> UserInterests => Set<UserInterest>();
		public DbSet<UserPhoto> UserPhotos => Set<UserPhoto>();
		public DbSet<AppUserFriend> Friends => Set<AppUserFriend>();
		public DbSet<Message> Messages => Set<Message>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<AppUserFriend>()
				.HasKey(k => new { k.AddingToFriendsUserId, k.AddedToFriendsUserId });

			modelBuilder.Entity<AppUserFriend>()
				.HasOne(s => s.AddingToFriendsUser)
				.WithMany(l => l.AddedToFriendsUsers)
				.HasForeignKey(s => s.AddingToFriendsUserId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<AppUserFriend>()
				.HasOne(s => s.AddedToFriendsUser)
				.WithMany(l => l.AddedToFriendsByUsers)
				.HasForeignKey(s => s.AddedToFriendsUserId)
				.OnDelete(DeleteBehavior.Cascade);
				
			modelBuilder.Entity<Message>()
				.HasOne(u => u.Recipient)
				.WithMany(m => m.MessagesRecieved)
				.OnDelete(DeleteBehavior.Restrict);
				
			modelBuilder.Entity<Message>()
				.HasOne(u => u.Sender)
				.WithMany(m => m.MessagesSent)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}