using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
	public static class IdentityServiceExtensions
	{
		public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddIdentityCore<AppUser>(options =>
			{
				options.Password.RequireNonAlphanumeric = false;
			})
				.AddRoles<AppRole>()
				.AddRoleManager<RoleManager<AppRole>>()
				.AddSignInManager<SignInManager<AppUser>>()
				.AddRoleValidator<RoleValidator<AppRole>>()
				.AddEntityFrameworkStores<DataContext>();

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"])),
						ValidateIssuer = false,
						ValidateAudience = false
					};
					
					options.Events = new JwtBearerEvents
					{
						OnMessageReceived = context => 
						{
							var accessToken = context.Request.Query["access_token"];
							var path = context.HttpContext.Request.Path;
							
							if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
							{
								context.Token = accessToken;
							}
							
							return Task.CompletedTask;
						}
					};
				});
				
			services.AddAuthorization(options => 
			{
				options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
				options.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
			});

			return services;
		}
	}
}