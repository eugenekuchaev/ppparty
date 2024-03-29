using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
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
                options.UseNpgsql(Environment.GetEnvironmentVariable("DatabaseDefaultConnection")!);
            });

            services.AddRouting(options =>
			{
				options.LowercaseUrls = true;
			});

            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<PresenceTracker>();

            services.AddScoped<LinkTransformer>();
            services.AddScoped<InputProcessor>();

            return services;
        }
    }
}