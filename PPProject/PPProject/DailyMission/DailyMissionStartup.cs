using PPProject.Common.InGame;
using PPProject.DailyMission.Service;

namespace PPProject.DailyMission
{
    public class DailyMissionStartup : IHostedService
    {
        private readonly UserActivityDispatcher _dispatcher;
        private readonly IServiceScopeFactory _scopeFactory;
        public DailyMissionStartup(
            UserActivityDispatcher dispatcher,
            IServiceScopeFactory scopeFactory)
        {
            _dispatcher = dispatcher;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken token)
        {
            //로그인 미션 진행
            _dispatcher.AddUserLoggedInListener(
                async uId => 
                {
                    using var scope = _scopeFactory.CreateScope();
                    var missionService = scope.ServiceProvider.GetRequiredService<DailyMissionService>();
                    await missionService.ProgressDailyMission(uId, 3, 1);
                } );
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
