using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions options) : base(options)
		{
        }

        public DbSet<AppUser>? Users { get; set; }	
		public DbSet<UserInterest>? UserInterests { get; set; }
		public DbSet<AppUserUserInterest>? AppUserUserInterests { get; set; }
		
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<AppUserUserInterest>()
				.HasKey(k => new { k.AppUserId, k.UserInterestId });

			builder.Entity<AppUserUserInterest>()
				.HasOne<AppUser>(o => o.AppUser)
				.WithMany(m => m.AppUserUserInterests)
				.HasForeignKey(fk => fk.AppUserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<AppUserUserInterest>()
				.HasOne<UserInterest>(o => o.UserInterest)
				.WithMany(m => m.AppUserUserInterests)
				.HasForeignKey(fk => fk.UserInterestId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}