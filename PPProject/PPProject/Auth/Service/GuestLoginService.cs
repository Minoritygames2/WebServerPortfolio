
using PPProject.Auth.DTO;
using PPProject.Auth.Interfaces;

namespace PPProject.Auth.Service
{
    public class GuestLoginService : ILoginService
    {
        private readonly UserRepository _repository;

        public GuestLoginService(UserRepository repository)
        {
            _repository = repository;
        }

        public async Task<LoginResult> LoginAsync(int platformCode, string platformUserId)
        {
            //DB에 유저 정보가 있는지 확인하고, 존재한다면 유저 정보를 반환
            var user = await _repository.GetByPlatformIdAsync(platformCode, platformUserId);
            if (user != null)
                return new LoginResult()
                {
                    UserId = user.uId,
                    Status = user.Status
                };

            //유저를 생성
            var newUser = await _repository.CreateAsync(platformCode, platformUserId);
            return new LoginResult()
            {
                UserId = newUser.uId,
                Status = newUser.Status
            };
        }
    }
}
