using PPProject.Resource.Infrastructure;
using PPProject.Resource.Service;

namespace PPProject.Resource
{
    public static class ResourceExtention
    {
        public static IServiceCollection AddResource(this IServiceCollection services)
        {
            services.AddScoped<ResourceService>();

            services.AddScoped<UserResourceRepository>();
            services.AddScoped<UserResourceHistoryRepository>();
            return services;
        }
    }
}
