using PPProject.Auth.Factories;
using PPProject.Auth.Interfaces;
using PPProject.Auth.Service;

namespace PPProject.Auth
{
    public static class AuthExtentions
    {
        public static IServiceCollection AddAuth(this IServiceCollection services)
        {
            services.AddTransient<GuestLoginService>();
            services.AddScoped<UserRepository>();
            services.AddScoped<LoginServiceFactory>();
            return services;
        }
    }
}
