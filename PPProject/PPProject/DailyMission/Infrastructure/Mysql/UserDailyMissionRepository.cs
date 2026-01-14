using Dapper;
using PPProject.DailyMission.Model;
using PPProject.Infrastructure.Mysql;

namespace PPProject.DailyMission.Infrastructure.Mysql
{
    public class UserDailyMissionRepository
    {
        private readonly MysqlSession _session;
        public UserDailyMissionRepository(MysqlSession session)
        {
            _session = session; 
        }

        public async Task<List<UserDailyMission>> GetUserMissions(
            long uId,
            DateOnly date
            )
        {
            const string SQL = @"
                SELECT
                    Id,
                    uId,
                    mission_id,
                    mission_date,
                    progress,
                    is_success,
                    completed_time,
                    created_time,
                    update_time
                FROM
                    UserDailyMission
                WHERE
                    uId = @UserId
                AND
                    mission_date = @MissionDate
                ";

            var result = await _session.Connection.QueryAsync<UserDailyMission>(SQL,
                new
                {
                    UserId = uId,
                    MissionDate = date
                },
                transaction: _session.Transaction);
            if (result == null)
                return new List<UserDailyMission>();
            return result.ToList();
        }
    }
}
