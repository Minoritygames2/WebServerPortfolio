using Dapper;
using PPProject.Infrastructure.Mysql;
using PPProject.Resource.Model;

namespace PPProject.Resource.Infrastructure
{
    public class UserResourceRepository
    {
        private readonly MysqlSession _session;
        public UserResourceRepository(MysqlSession session)
        {
            _session = session;
        }

        public async Task<int> CreateResource(
            long uId,
            long resourceId,
            int amount
            )
        {
            const string SQL = @"
                INSERT INTO UserResource (
                    uId,
                    resource_id,
                    amount,
                    created_time,
                    update_time
                ) VALUES (
                    @uId,
                    @resourceId,
                    @amount,
                    NOW(),
                    NOW()
                );";

            await _session.Connection.ExecuteAsync( SQL, new 
            {
                uId,
                resourceId,
                amount
            },
            transaction: _session.Transaction);

            return amount;
        }

        public async Task<int?> GetAmountAsync(long uId, int resourceId)
        {
            const string SQL = @"
                SELECT
                    amount
                FROM UserResource
                WHERE
                    uId = @uId
                AND
                    resource_id = @resourceId
                LIMIT 1;";

            return await _session.Connection.QueryFirstOrDefaultAsync<int?>(
                SQL,
                new { uId, resourceId },
                transaction: _session.Transaction);
        }

        public async Task<Dictionary<int, int>> GetAmountsAsync(long uId,
          IEnumerable<int> resourceIds)
        {
            const string SQL = @"
                SELECT
                    resource_id,
                    amount
                FROM UserResource
                WHERE
                    uId = @uId
                AND
                    resource_id IN @resourceIds
                LIMIT 1;";

            var result = await _session.Connection.QueryAsync<(int resourceId, int amount)>(
                SQL,
                new { uId, resourceIds },
                transaction: _session.Transaction);

            return result.ToDictionary(_=>_.resourceId, _=>_.amount);
        }

        public async Task<bool> TryAddAsync(
            long uId,
            int resourceId,
            int addCost,
            int maxCount)
        {
            var parameters = new DynamicParameters();
            parameters.Add("uId", uId);
            parameters.Add("resourceId", resourceId);
            parameters.Add("addCost", addCost);

            string SQL = @"
                UPDATE UserResource
                SET
                    amount = amount + @addCost,
                    update_time = NOW()
                WHERE
                    uId = @uId
                AND
                    resource_id = @resourceId";

            if (maxCount > 0)
            {
                SQL += @"
                AND 
                    amount + @addCost <= @maxCount";

                parameters.Add("maxCount", maxCount);
            }

            var affectedRows = await _session.Connection.ExecuteAsync(
                SQL,
                parameters,
                transaction: _session.Transaction);

            return affectedRows > 0;
        }


        public async Task<bool> TryConsumeAsync(
            long uId,
            int resourceId,
            int minusCost)
        {
            const string SQL = @"
                UPDATE UserResource
                SET
                    amount = amount - @minusCost,
                    update_time = NOW()
                WHERE
                    uId = @uId
                AND
                    resource_id = @resourceId
                AND
                    @minusCost <= amount 
                AND 
                    0 <= @minusCost";

            var affectedRows = await _session.Connection.ExecuteAsync(
                SQL,
                new
                {
                    uId,
                    resourceId,
                    minusCost
                },
                transaction: _session.Transaction);

            return affectedRows > 0;
        }
    }
}
