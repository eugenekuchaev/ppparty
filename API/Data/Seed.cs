using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	public class Seed
	{
		public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
		{
			if (await userManager.Users.AnyAsync()) return;

			var userData = await File.ReadAllTextAsync("Data/AppUserSeedData.json");
			var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
			
			var roles = new List<AppRole>
			{
				new AppRole{ Name = "Member" },
				new AppRole{ Name = "Admin" },
				new AppRole{ Name = "Moderator" }
			};
			
			foreach (var role in roles)
			{
				await roleManager.CreateAsync(role);
			}

			foreach (var user in users!)
			{
				user.UserName = user.UserName.ToLower();
				await userManager.CreateAsync(user, "Pa$$w0rd");
				await userManager.AddToRoleAsync(user, "Member");
			}
			
			var admin = new AppUser
			{
				UserName = "admin",
				FullName = "Admin"
			};
			
			admin.UserPhoto = new UserPhoto 
			{
				PhotoUrl = "https://res.cloudinary.com/duy1fjz1z/image/upload/v1678110186/user_epf5zu.png"
			};
			
			await userManager.CreateAsync(admin, "Pa$$w0rd");
			await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
		}
	}
}