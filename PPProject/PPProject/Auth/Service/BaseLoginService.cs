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

        public abstract Task<string> VerifyPlatformItemtityAsync(string platformUserId);
        

        
    }
}
