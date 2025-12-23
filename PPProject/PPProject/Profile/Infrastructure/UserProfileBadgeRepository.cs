using Dapper;
using MySqlConnector;
using PPProject.Infrastructure.Mysql;
using PPProject.Profile.Model;

namespace PPProject.Profile.Infrastructure
{
    public class UserProfileBadgeRepository
    {
        private readonly MysqlSession _session;
        public UserProfileBadgeRepository(MysqlSession session)
        {
            _session = session;
        }
        public async Task<List<UserProfileBadge>> GetUserBadgesByUidAsync(long uId)
        {
            const string SQL = @"
                SELECT 
                    uId, 
                    slot_index, 
                    badge_type, 
                    badge_id, 
                    created_time, 
                    update_time 
                FROM
                    UserProfileBadge 
                WHERE 
                    uId = @uId
                ";
            return await _session.Connection.QueryAsync<UserProfileBadge>(SQL, new { uId },
            transaction: _session.Transaction) 
                is var result
                ? result.ToList()
                : new List<UserProfileBadge>();
        }
    }
}
