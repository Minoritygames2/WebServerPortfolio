using Dapper; 
using IdGen;
using MySqlConnector;
using PPProject.Auth.Model;
using PPProject.Infrastructure.Mysql;

namespace PPProject.Auth.Infrastructure
{
    public class UserRepository
    {
        private readonly MysqlSession _session;
        private readonly IdGenerator _idGenerator;

        public UserRepository(MysqlSession session, IdGenerator idGenerator)
        {
            _session = session;
            _idGenerator = idGenerator;
        }

        public async Task<User?> GetByPlatformIdAsync(int platformCode, string platformUserId)
        {
            const string SQL = @"
                SELECT
                    uId,
                    platformCode,
                    platformUserId,
                    Status,
                    createdTime,
                    updateTime
                FROM Users
                WHERE
                    platformCode = @platformCode
                AND
                    platformUserId = @platformUserId
                LIMIT 1;";

            return await _session.Connection.QueryFirstOrDefaultAsync<User>(SQL, new
            {
                platformCode,
                platformUserId
            },
            transaction: _session.Transaction);
        }

        public async Task<User> CreateAsync(int platformCode, string platformUserId)
        {
            long uId = _idGenerator.CreateId();
            const string SQL = @"
                INSERT INTO Users (
                    uId,
                    platformCode,
                    platformUserId,
                    Status,
                    createdTime,
                    updateTime
                ) VALUES (
                    @uId,
                    @platformCode,
                    @platformUserId,
                    1,
                    NOW(),
                    NOW()
                );";

            await _session.Connection.ExecuteAsync(SQL, new
            {
                uId,
                platformCode,
                platformUserId
            },
            transaction: _session.Transaction);
            return new User
            {
                uId = uId,
                platformCode = platformCode,
                platformUserId = platformUserId,
                Status = 1,
                createdTime = DateTime.Now,
                updateTime = DateTime.Now
            };
        }
    }
}
