using Dapper; 
using IdGen;
using MySqlConnector;
using PPProject.Auth.Model;

namespace PPProject.Auth.Interfaces
{
    public class UserRepository
    {
        private readonly MySqlConnection _conn;
        private readonly IdGenerator _idGenerator;

        public UserRepository(MySqlConnection conn, IdGenerator idGenerator)
        {
            _conn = conn;
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

            return await _conn.QueryFirstOrDefaultAsync<User>(SQL, new
            {
                platformCode,
                platformUserId
            });
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

            await _conn.ExecuteAsync(SQL, new
            {
                uId,
                platformCode,
                platformUserId
            });
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
