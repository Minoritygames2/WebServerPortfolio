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
                    mission_id AS MissionId,
                    mission_date AS MissionDate,
                    progress AS Progress,
                    is_success AS IsSuccess,
                    completed_time AS CompletedTime,
                    receive_reward AS ReceiveReward,
                    receive_time AS ReceiveTime,
                    created_time AS CreatedTime,
                    update_time AS UpdateTime
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

        public async Task<bool> AddProgressUserMissions(
            long uId,
            int missionId,
            DateOnly date,
            int addProgress,
            int successValue
            )
        {
            const string SQL = @"
                INSERT INTO UserDailyMission (
                    uId,
                    mission_id,
                    mission_date,
                    progress,
                    is_success,
                    completed_time,
                    receive_reward,
                    receive_time,
                    created_time,
                    update_time
                )
                VALUES (
                    @uId,
                    @missionId,
                    @missionDate,
                    0,
                    0,
                    NULL,
                    0,
                    NULL,
                    NOW(),
                    NOW()
                )
                ON DUPLICATE KEY UPDATE
                    progress = CASE
                        WHEN is_success = 1 THEN progress
                        ELSE progress + @addProgress
                END,
                is_success = CASE
                    WHEN is_success = 1 THEN is_success
                    WHEN progress + @addProgress >= @successValue THEN 1
                    ELSE 0
                END,
                completed_time = CASE
                    WHEN completed_time IS NOT NULL THEN completed_time
                    WHEN is_success = 1
                        THEN NOW()
                    ELSE NULL
                END,
                update_time = NOW()
            ";

            var affectedRows = await _session.Connection.ExecuteAsync(SQL,
                new
                {
                    uId = uId,
                    missionId = missionId,
                    missionDate = date,
                    addProgress = addProgress,
                    successValue = successValue
                },
                transaction: _session.Transaction);

            return affectedRows > 0;
        }
    }
}
