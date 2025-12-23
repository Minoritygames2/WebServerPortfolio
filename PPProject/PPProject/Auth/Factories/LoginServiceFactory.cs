using PPProject.Auth.Service;
using PPProject.Common;

namespace PPProject.Auth.Factories
{
    public class LoginServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public LoginServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public BaseLoginService GetService(int? platformCode)
        {
            return platformCode switch
            {
                (int)PlatformType.Guest => _serviceProvider.GetRequiredService<GuestLoginService>(),
                (int)PlatformType.Google => _serviceProvider.GetRequiredService<GoogleLoginService>(),
                _ => throw new NotSupportedException($"Platform code Error :  {platformCode}")
            };
        }
    }
}
