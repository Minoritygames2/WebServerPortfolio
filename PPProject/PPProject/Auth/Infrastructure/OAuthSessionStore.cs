using PPProject.Auth.DTO;
using StackExchange.Redis;
using System.Text.Json;

namespace PPProject.Auth.Infrastructure
{
    public class OAuthSessionStore
    {
        private readonly IDatabase _db;
        private const string KeyPrefix = "oauth:session";

        public OAuthSessionStore(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        private string BuildKey(string sessionId)
            => $"{KeyPrefix}:{sessionId}";

        public async Task SaveAuthSession(
            string sessionId, 
            string idToken, 
            string authToken, 
            int platform, 
            TimeSpan ttl)
        {
            var oauthInfo = new OAuthInfo()
            {
                IdToken = idToken,
                AuthCode = authToken,
                Platform  = platform
            };
            await SetAsync(sessionId, oauthInfo, ttl);
        }

        public async Task<string> GetAuthIdToken(string sessionId)
        {
            var result = await GetAsync(sessionId);
            return result.IdToken;
        }

        public Task<bool> IsHasKey(string sessionId)
        {
            var key = BuildKey(sessionId);
            return _db.KeyExistsAsync(key);
        }

        private async Task SetAsync(string sessionId, OAuthInfo oauthInfo, TimeSpan ttl)
        {
            var key = BuildKey(sessionId);
            var data = JsonSerializer.Serialize(oauthInfo);
            await _db.StringSetAsync(key, data, ttl);
        }

        private async Task<OAuthInfo?> GetAsync(string sessionId)
        {
            var key = BuildKey(sessionId);
            var result = await _db.StringGetAsync(key);
            if (result.IsNullOrEmpty) return null;

            var objResult = JsonSerializer.Deserialize<OAuthInfo>(result.ToString());
            return objResult;
        }

        private async Task RemoveAsync(string sessionId)
        {
            var key = BuildKey(sessionId);
            await _db.KeyDeleteAsync(key);
        }
    }
}
