using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
	public static class ApplicationServiceExtensions
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services, 
			IConfiguration configuration)
		{
			services.AddDbContext<DataContext>(options => 
			{
				options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
			});
			
			services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
			services.AddScoped<IPhotoService, PhotoService>();
			
			services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
			
			services.AddScoped<ITokenService, TokenService>();

			services.AddScoped<IUserRepository, UserRepository>();
			
			return services;
		}
	}
}