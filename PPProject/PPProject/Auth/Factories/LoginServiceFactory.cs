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

        public ILoginService GetService(int? platformCode)
        {
            return platformCode switch
            {
                (int)PlatformType.Guest => _serviceProvider.GetRequiredService<GuestLoginService>(),
                _ => throw new NotSupportedException($"Platform code Error :  {platformCode}")
            };
        }
    }
}
