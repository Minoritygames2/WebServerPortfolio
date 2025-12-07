using Microsoft.AspNetCore.Mvc;
using PPProject.Auth.DTO.Request;
using PPProject.Auth.DTO.Response;
using PPProject.Auth.Factories;
using PPProject.Common;
using PPProject.Filters;

namespace PPProject.Auth.Controllers
{
    [ApiController]
    [Route("auth")]
    [ValidateParams]
    public class AuthController : ControllerBase
    {
        private readonly LoginServiceFactory _loginServiceFactory;
        public AuthController(LoginServiceFactory loginServiceFactory)
        {
            _loginServiceFactory = loginServiceFactory;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody]LoginRequest request)
        {
            try
            {
                var service = _loginServiceFactory.GetService(request.PlatformCode);
                var result = await service.LoginAsync(request.PlatformCode, request.UserCode);
                var response = new LoginResponse
                {
                    Session = 0,
                    UserId = result.UserId,
                    Status = result.Status
                };
                return ApiResponse<LoginResponse>.Success(response);
            }
            catch (NotSupportedException ex)
            {
                return ApiResponse<LoginResponse>.Error(ErrorCode.NOT_SUPPORTED_PLATFORM, ex.Message);
            }
            catch (Exception ex)
            {
                return ApiResponse<LoginResponse>.Error(5000, ex.Message);
            }
            
        }
    }
}
