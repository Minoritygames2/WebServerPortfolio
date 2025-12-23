using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PPProject.Auth.DTO.Request;
using PPProject.Auth.DTO.Response;
using PPProject.Auth.Factories;
using PPProject.Auth.Service;
using PPProject.Common;
using PPProject.Filters;
using PPProject.Profile.Service;
using PPProject.Usecase.CreateUser;

namespace PPProject.Auth.Controllers
{
    [ApiController]
    [Route("auth")]
    [ValidateParams]
    public class AuthController : ControllerBase
    {
        private readonly LoginServiceFactory _loginServiceFactory;
        private readonly AuthService _authService;
        private readonly GoogleAuthWindowService _windowService;
        private readonly CreateUserUseCase _createUserUseCase;
        public AuthController(LoginServiceFactory loginServiceFactory
            , AuthService authService
            , GoogleAuthWindowService windowService
            , CreateUserUseCase createUserUseCase)
        {
            _loginServiceFactory = loginServiceFactory;
            _authService = authService;
            _windowService = windowService;
            _createUserUseCase = createUserUseCase;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                //플랫폼별 인증
                var loginService = _loginServiceFactory.GetService(request.PlatformCode);
                var platformUserId = await loginService.VerifyPlatformItemtityAsync(request.UserCode);

                //유저 생성 및 조회
                var result = await _createUserUseCase.GetAndCreateUserAsync(request.PlatformCode, platformUserId);

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

        [HttpGet("window/start")]
        public async Task<ActionResult<ApiResponse<WindowAuthResponse>>> WindowAuthStart()
        {
            var sessionId = Guid.NewGuid().ToString();
            var authUrl = _windowService.CreateWindowAuthUrl(sessionId);
            var response = new WindowAuthResponse()
            {
                AuthUrl = authUrl,
                SessionId = sessionId
            };
            return ApiResponse<WindowAuthResponse>.Success(response);
        }

        [HttpGet("window/callback")]
        public async Task<ActionResult<ApiResponse<WindowAuthCallbackResponse>>> WindowAuthCallback(
            [FromQuery] string state,
            [FromQuery] string code)
        {
            try
            {
                var idToken = await _windowService.RequestOAuthTokenAsync(code);

                if (string.IsNullOrEmpty(idToken))
                    return ApiResponse<WindowAuthCallbackResponse>.Error(ErrorCode.GOOGLE_AUTH_VALIDATION_FAILED, "IdToken is null");

                await _windowService.SaveAuthSession(state, idToken);

                var response = new WindowAuthCallbackResponse()
                {
                    Token = idToken
                };

                return ApiResponse<WindowAuthCallbackResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<WindowAuthCallbackResponse>.Error(5000, ex.Message);
            }
        }


        [HttpGet("window/result")]
        public async Task<ActionResult<ApiResponse<WindowAuthResultResponse>>> WindowAuthResult(
            [FromQuery] string sessionId)
        {
            try
            {
                var isHasKey = await _windowService.HasAuthSession(sessionId);
                var response = new WindowAuthResultResponse();
                if (isHasKey)
                {
                    var idToken = await _windowService.GetAuthIdToken(sessionId);
                    response.Status = 1;
                    response.IdToken = idToken;
                }
                else
                {
                    response.Status = 0;
                }

                return ApiResponse<WindowAuthResultResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<WindowAuthResultResponse>.Error(5000, ex.Message);
            }
        }
    }
}
