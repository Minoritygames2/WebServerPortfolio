using PPProject.Profile.Infrastructure;
using PPProject.Profile.Service;

namespace PPProject.Profile
{
    public static class ProfileExtentions
    {
        public static IServiceCollection AddProfile(this IServiceCollection services)
        {
            services.AddScoped<UserProfileService>();
            services.AddScoped<UserProfileBadgeService>();

            services.AddScoped<UserProfileRepository>();
            services.AddScoped<UserProfileBadgeRepository>();
            return services;
        }
    }
}
