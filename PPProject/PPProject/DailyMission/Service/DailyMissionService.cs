using PPProject.DailyMission.DTO;
using PPProject.DailyMission.Infrastructure.Mysql;
using PPProject.DailyMission.Infrastructure.Redis;
using System.Threading.Tasks;

namespace PPProject.DailyMission.Service
{
    public class DailyMissionService
    {
        private readonly DailyMissionStore _dailyMissionStore;
        private readonly UserDailyMissionRepository _userRepository;

        public DailyMissionService(DailyMissionStore store,
            UserDailyMissionRepository userRepository)
        {
            _dailyMissionStore = store;
            _userRepository = userRepository;
        }


        public async Task<List<DailyMissionInfo>> GetDailyMission(long uId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            //데일리 미션 테이블
            var storedMissions = await _dailyMissionStore.GetStoreDailyMissionsAsync();


            //유저의 미션 정보
            var userMissions = await _userRepository.GetUserMissions(uId, today);

            var userMissionMap = userMissions.ToDictionary(x => x.MissionId);

            var result = new List<DailyMissionInfo>();

            foreach ( var storedMission in storedMissions)
            {
                userMissionMap.TryGetValue(storedMission.Id, out var userMission);

                result.Add(new DailyMissionInfo {
                    MissionId = storedMission.Id,
                    MissionType = storedMission.MissionType,
                    IsSuccess = userMission?.IsSuccess ?? false,
                    SuccessValue = userMission?.Progress ?? 0
                });
            }
            return result;
        }
    }
}
