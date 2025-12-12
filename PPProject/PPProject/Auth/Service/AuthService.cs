using IdGen;
using PPProject.Common.Session;
using System.Reflection.Emit;

namespace PPProject.Auth.Service
{
    public class AuthService
    {
        private readonly RedisGameSessionStore _redisSessionStore;
        private readonly IdGenerator _idGenerator;
        public AuthService(RedisGameSessionStore redisSessionStore, IdGenerator idGenerator)
        {
            _redisSessionStore = redisSessionStore;
            _idGenerator = idGenerator;
        }

        public async Task<long> CreateSession(long uId)
        {
            var sessionId = _idGenerator.CreateId();
            await _redisSessionStore.SaveSession(uId, sessionId);
            return sessionId;
        }

    }
}
