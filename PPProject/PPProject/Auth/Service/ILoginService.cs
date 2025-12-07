using PPProject.Auth.DTO;

namespace PPProject.Auth.Service
{
    public interface ILoginService
    {
        Task<LoginResult> LoginAsync(int platformCode, string platformUserId);
    }
}
