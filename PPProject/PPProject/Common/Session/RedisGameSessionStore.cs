using StackExchange.Redis;
using System.Text.Json;

namespace PPProject.Common.Session
{
    public class RedisGameSessionStore
    {
        private readonly IDatabase _db;
        private const string KeyPrefix = "game:session";

        public RedisGameSessionStore(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        private string BuildKey(string uid)
            => $"{KeyPrefix}:{uid}";

        public async Task<long> GetSessionAsync(long uId)
        {
            var getResult = await GetAsync(uId);
            if (getResult == null)
                throw new Exception("Session Is Null");
            return getResult.Session;
        }

        public async Task SaveSession(long uId, long sessionId)
        {
            var data = new GameSessionDTO() {
                UID = uId,
                Session = sessionId,
                LastActiveAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            var ttl = Environment.GetEnvironmentVariable("GAME_SESSION_SAVE_TIME");
            var ttlDouble = double.Parse(ttl);
            var ttlSpan = TimeSpan.FromSeconds(ttlDouble);
            await SetAsync(uId, data, ttlSpan);
        }

        public async Task RefreshTTL(long uId)
        {
            var sessionInfo = await GetAsync(uId);
            if (sessionInfo == null) //세션값이 없다면 갱신하지않음
                return;

            //타임 갱신
            sessionInfo.LastActiveAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            //TTL 로드
            var ttl = Environment.GetEnvironmentVariable("GAME_SESSION_SAVE_TIME");
            var ttlDouble = double.Parse(ttl);
            var ttlSpan = TimeSpan.FromSeconds(ttlDouble);

            await SetAsync(uId, sessionInfo, ttlSpan);
        }

        public async Task DeleteSession(long uId)
        {
            var uidStr = uId.ToString();
            var key = BuildKey(uidStr);
            await _db.KeyDeleteAsync(key);
        }

        private async Task<GameSessionDTO> GetAsync(long uId)
        {
            var key = BuildKey(uId.ToString());
            var result = await _db.StringGetAsync(key);

            if (result.IsNullOrEmpty) return null;

            var objResult = JsonSerializer.Deserialize<GameSessionDTO>(result.ToString());
            return objResult;
        }

        private async Task SetAsync(long uId, GameSessionDTO dtoData, TimeSpan ttl)
        {
            var uidStr = uId.ToString();
            var key = BuildKey(uidStr);
            var data = JsonSerializer.Serialize(dtoData);
            await _db.StringSetAsync(key, data, ttl);
        }
    }
}
