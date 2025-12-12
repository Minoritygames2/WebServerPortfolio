using Microsoft.AspNetCore.Mvc;
using PPProject.Auth.DTO.Request;
using PPProject.Auth.DTO.Response;
using PPProject.Auth.Factories;
using PPProject.Auth.Service;
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
        private readonly AuthService _authService;
        public AuthController(LoginServiceFactory loginServiceFactory, AuthService authService)
        {
            _loginServiceFactory = loginServiceFactory;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody]LoginRequest request)
        {
            try
            {
                var loginService = _loginServiceFactory.GetService(request.PlatformCode);
                var result = await loginService.LoginAsync(request.PlatformCode, request.UserCode);
                
                //세션키 생성
                var session = await _authService.CreateSession(result.UserId);
                
                var response = new LoginResponse
                {
                    Session = session,
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

        [HttpPost("Ok")]
        public async Task<ActionResult<ApiResponse<string>>> OK()
        {
            return ApiResponse<string>.Success("OK");
        }
    }
}
