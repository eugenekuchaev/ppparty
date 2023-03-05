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
	}
}