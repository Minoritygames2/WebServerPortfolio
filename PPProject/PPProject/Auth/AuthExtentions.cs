using PPProject.Auth.Factories;
using PPProject.Auth.Infrastructure;
using PPProject.Auth.Service;

namespace PPProject.Auth
{
    public static class AuthExtentions
    {
        public static IServiceCollection AddAuth(this IServiceCollection services)
        {
            services.AddScoped<GuestLoginService>();
            services.AddScoped<GoogleLoginService>();

            services.AddScoped<AuthService>();
            services.AddScoped<GoogleAuthWindowService>();

            services.AddScoped<UserRepository>();
            services.AddScoped<LoginServiceFactory>();
            services.AddSingleton<OAuthSessionStore>();
            return services;
        }
    }
}
