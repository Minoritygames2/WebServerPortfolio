using PPProject.DailyMission.Infrastructure.Mysql;
using PPProject.DailyMission.Infrastructure.Redis;
using PPProject.DailyMission.Service;

namespace PPProject.DailyMission
{
    public static class DailyMissionExtentions
    {
        public static IServiceCollection AddDailyMission(this IServiceCollection services)
        {
            services.AddScoped<DailyMissionService>();

            services.AddScoped<DailyMissionStore>();
            services.AddScoped<UserDailyMissionRepository>();

            services.AddHostedService<DailyMissionStartup>();
            return services;
        }
    }
}
