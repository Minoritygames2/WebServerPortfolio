using Dapper;
using PPProject.Infrastructure.Mysql;

namespace PPProject.Resource.Infrastructure
{
    public class UserResourceHistoryRepository
    {
        private readonly MysqlSession _session;

        public UserResourceHistoryRepository(MysqlSession session)
        {
            _session = session;
        }

        public async Task CreateHistory(
            long uId,
            long resourceId,
            int changeAmount,
            int beforeAmount,
            int afterAmount,
            int reasonCode,
            string reasonMsg
            )
        {
            const string SQL = @"
                INSERT INTO UserResourceHistory (
                    uId,
                    resource_id,
                    change_amount,
                    before_amount,
                    after_amount,
                    reason_code,
                    reason_msg,
                    created_time
                ) VALUES (
                    @uId,
                    @resourceId,
                    @changeAmount,
                    @beforeAmount,
                    @afterAmount,
                    @reasonCode,
                    @reasonMsg,
                    NOW()
                );";
            await _session.Connection.ExecuteAsync(SQL, new {
                uId,
                resourceId,
                changeAmount,
                beforeAmount,
                afterAmount,
                reasonCode,
                reasonMsg
            },
            transaction: _session.Transaction);

        }
    }
}
