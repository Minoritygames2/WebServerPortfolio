using IdGen;
using PPProject.Auth.DTO;
using PPProject.Auth.Infrastructure;
using PPProject.Common.Session;

namespace PPProject.Auth.Service
{
    public class AuthService
    {
        private readonly RedisGameSessionStore _redisSessionStore;
        private readonly IdGenerator _idGenerator;
        private readonly UserRepository _userRepository;
        public AuthService(RedisGameSessionStore redisSessionStore, IdGenerator idGenerator, 
            UserRepository userRepository)
        {
            _redisSessionStore = redisSessionStore;
            _idGenerator = idGenerator;
            _userRepository = userRepository;
        }

        public async Task<long> CreateSession(long uId)
        {
            var sessionId = _idGenerator.CreateId();
            await _redisSessionStore.SaveSession(uId, sessionId);
            return sessionId;
        }

        public virtual async Task<LoginResult> GetUserByPlatform(int platformCode, string platformUserId)
        {
            //DB에 유저 정보가 있는지 확인하고, 존재한다면 유저 정보를 반환
            var user = await _userRepository.GetByPlatformIdAsync(platformCode, platformUserId);
            if (user != null)
            {
                return new LoginResult()
                {
                    UserId = user.uId,
                    Status = user.Status,
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<LoginResult?> CreateUser(int platformCode, string platformUserId)
        {
            //유저를 생성
            var newUser = await _userRepository.CreateAsync(platformCode, platformUserId);
            return new LoginResult()
            {
                UserId = newUser.uId,
                Status = newUser.Status
            };
        }

    }
}
