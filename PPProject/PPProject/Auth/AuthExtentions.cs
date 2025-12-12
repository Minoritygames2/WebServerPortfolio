using PPProject.Auth.Factories;
using PPProject.Auth.Infrastructure;
using PPProject.Auth.Service;

namespace PPProject.Auth
{
    public static class AuthExtentions
    {
        public static IServiceCollection AddAuth(this IServiceCollection services)
        {
            services.AddTransient<GuestLoginService>();
            services.AddTransient<GoogleLoginService>();
            services.AddTransient<AuthService>();
            services.AddScoped<UserRepository>();
            services.AddScoped<LoginServiceFactory>();
            services.AddSingleton<OAuthSessionStore>();
            return services;
        }
    }
}
