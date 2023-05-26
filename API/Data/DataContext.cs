using System.Reflection;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data
{
	public class DataContext : IdentityDbContext<AppUser, AppRole, int,
		IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
		IdentityRoleClaim<int>, IdentityUserToken<int>>
	{
		public DataContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<UserInterest> UserInterests => Set<UserInterest>();
		public DbSet<UserPhoto> UserPhotos => Set<UserPhoto>();
		public DbSet<AppUserFriend> Friends => Set<AppUserFriend>();
		public DbSet<Message> Messages => Set<Message>();
		public DbSet<Group> Groups => Set<Group>();
		public DbSet<Connection> Connections => Set<Connection>();
		public DbSet<Event> Events => Set<Event>();
		public DbSet<EventPhoto> EventPhotos => Set<EventPhoto>();
		public DbSet<EventTag> EventTags => Set<EventTag>();
		public DbSet<EventNotification> EventNotifications => Set<EventNotification>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<AppUser>()
				.HasMany(ur => ur.UserRoles)
				.WithOne(u => u.User)
				.HasForeignKey(ur => ur.UserId)
				.IsRequired();

			modelBuilder.Entity<AppRole>()
				.HasMany(ur => ur.UserRoles)
				.WithOne(u => u.Role)
				.HasForeignKey(ur => ur.RoleId)
				.IsRequired();

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

			modelBuilder.Entity<AppUser>()
				.HasMany(e => e.OwnedEvents)
				.WithOne(e => e.EventOwner)
				.HasForeignKey(e => e.EventOwnerId)
				.IsRequired();

			modelBuilder.Entity<Event>()
				.HasMany(e => e.Participants)
				.WithMany(e => e.ParticipateInEvents);

			modelBuilder.Entity<Event>()
				.HasMany(e => e.InvitedToEvent)
				.WithMany(e => e.InvitedToEvents);

			modelBuilder.Entity<EventNotification>()
				.HasMany(e => e.Recipients)
				.WithMany(e => e.EventNotifications);

			modelBuilder.ApplyUtcDateTimeConverter();
		}
	}

	public static class UtcDateAnnotation
	{
		private const string IsUtcAnnotation = "IsUtc";
		private static readonly ValueConverter<DateTime, DateTime> UtcConverter = new ValueConverter<DateTime, DateTime>(convertTo => DateTime.SpecifyKind(convertTo, DateTimeKind.Utc), convertFrom => convertFrom);
		public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, bool isUtc = true) => builder.HasAnnotation(IsUtcAnnotation, isUtc);
		public static bool IsUtc(this IMutableProperty property)
		{
			if (property != null && property.PropertyInfo != null)
			{
				var attribute = property.PropertyInfo.GetCustomAttribute<IsUtcAttribute>();
				if (attribute is not null && attribute.IsUtc)
				{
					return true;
				}
				return ((bool?)property.FindAnnotation(IsUtcAnnotation)?.Value) ?? true;
			}
			return true;
		}
		public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
		{
			foreach (var entityType in builder.Model.GetEntityTypes())
			{
				foreach (var property in entityType.GetProperties())
				{
					if (!property.IsUtc())
					{
						continue;
					}
					if (property.ClrType == typeof(DateTime) ||
						property.ClrType == typeof(DateTime?))
					{
						property.SetValueConverter(UtcConverter);
					}
				}
			}
		}
	}
	
	public class IsUtcAttribute : Attribute
	{
		public IsUtcAttribute(bool isUtc = true) => this.IsUtc = isUtc;
		public bool IsUtc { get; }
	}
}