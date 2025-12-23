using PPProject.Auth.Infrastructure;

namespace PPProject.Auth.Service
{
    public class GuestLoginService : BaseLoginService
    {
        public GuestLoginService(UserRepository userRepository) : base(userRepository)
        {
            
        }

        public override Task<string> VerifyPlatformItemtityAsync(string platformUserId)
        {
            return Task.FromResult(platformUserId);
        }
    }
}
