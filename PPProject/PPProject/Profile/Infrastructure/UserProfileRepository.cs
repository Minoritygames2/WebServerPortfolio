using MySqlConnector;
using Dapper;
using PPProject.Profile.Model;
using PPProject.Infrastructure.Mysql;

namespace PPProject.Profile.Infrastructure
{
    public class UserProfileRepository
    {
        private readonly MysqlSession _session;
        public UserProfileRepository(MysqlSession session)
        {
            _session = session;
        }

        public async Task CreateProfileAsync(
            long uId, 
            string nickname, 
            int charId,
            int labelId,
            string message
            )
        {
            const string SQL = @"
                INSERT INTO UserProfile (
                    uId,
                    nickname,
                    char_id,
                    label_id,
                    message,
                    created_time,
                    update_time
                ) VALUES (
                    @uId,
                    @nickname,
                    @charId,
                    @labelId,
                    @message,
                    NOW(),
                    NOW()
                );";
            await _session.Connection.ExecuteAsync(SQL, new
            {
                uId,
                nickname,
                charId,
                labelId,
                message
            },
            transaction: _session.Transaction);
        }

        public async Task<UserProfile?> GetByUidAsync(long uId)
        {
            const string SQL = @"
                SELECT
                    uId,
                    nickname,
                    char_id,
                    label_id,
                    message,
                    created_time,
                    update_time
                FROM UserProfile
                WHERE
                    uId = @uId
                LIMIT 1;";
            return await _session.Connection.QueryFirstOrDefaultAsync<UserProfile>(SQL, new{uId},
            transaction: _session.Transaction);
        }

        public async Task<bool> UpdateUserProfileAsync(
            long uId,
            bool isChangeNickName,
            string nickName,
            bool isChangeLabel,
            int labelId,
            bool isChangeMessage,
            string message
            )
        {
            var setClauses = new List<string>();
            var parameters = new DynamicParameters();

            //UID 추가
            parameters.Add("uId", uId);

            //변경된 파라미터 추가
            if (isChangeNickName)
            {
                setClauses.Add("Nickname = @nickname");
                parameters.Add("nickName", nickName);
            }
            if (isChangeLabel)
            {
                setClauses.Add("LabelId = @label_id");
                parameters.Add("labelId", labelId);
            }
            if (isChangeMessage)
            {
                setClauses.Add("Message = @message");
                parameters.Add("message", message);
            }
            if (setClauses.Count == 0)
            {
                return false; 
            }
            setClauses.Add("update_time = NOW()");


            var setClause = string.Join(", ", setClauses);
            var SQL = $@"
                UPDATE UserProfile
                SET {setClause}
                WHERE uId = @uId;";


            var result = await _session.Connection.ExecuteAsync(SQL, parameters,
            transaction: _session.Transaction);
            return result > 0;
        }
    }
}
