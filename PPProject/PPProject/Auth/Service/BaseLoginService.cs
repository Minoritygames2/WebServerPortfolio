using PPProject.Auth.DTO;
using PPProject.Auth.Infrastructure;

namespace PPProject.Auth.Service
{
    public abstract class BaseLoginService
    {
        protected readonly UserRepository _userRepository;
        protected BaseLoginService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public virtual async Task<LoginResult> LoginAsync(int platformCode, string platformUserId)
        {
            //DB에 유저 정보가 있는지 확인하고, 존재한다면 유저 정보를 반환
            var user = await _userRepository.GetByPlatformIdAsync(platformCode, platformUserId);
            if (user != null)
                return new LoginResult()
                {
                    UserId = user.uId,
                    Status = user.Status
                };

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
