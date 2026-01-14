using PPProject.Auth.DTO;
using PPProject.Auth.Service;
using PPProject.Common.InGame;
using PPProject.Infrastructure.Mysql;
using PPProject.Profile.Service;

namespace PPProject.Usecase.CreateUser
{
    public class CreateUserUseCase
    {
        private readonly MysqlSession _mysqlSession;
        private readonly AuthService _authService;
        private readonly UserProfileService _userProfileService;

        private readonly UserActivityDispatcher _userActivityDispatcher;

        public CreateUserUseCase(
            MysqlSession mysqlSession,
            AuthService authService,
            UserProfileService userProfileService,
            UserActivityDispatcher userActivityDispatcher)
        {
            _mysqlSession = mysqlSession;
            _authService = authService;
            _userProfileService = userProfileService;
            _userActivityDispatcher = userActivityDispatcher;
        }

        public async Task<LoginResult> GetAndCreateUserAsync(int platformCode, string platformUserId)
        {
            try
            {
                _mysqlSession.BeginTransaction();
                var loginResult = await _authService.GetUserByPlatform(platformCode, platformUserId);
                if (loginResult == null)
                {
                    //유저 생성
                    loginResult = await _authService.CreateUser(platformCode, platformUserId);
                    //유저 프로필 생성
                    await _userProfileService.CreateDefaultProfile(loginResult.UserId);
                }

                //유저 로그인 Dispatcher
                await _userActivityDispatcher.OnUserLoggedIn(loginResult.UserId);

                _mysqlSession.Commit();
                return loginResult;
            }
            catch
            {
                _mysqlSession.Rollback();
                throw;
            }
        }
    }
}
