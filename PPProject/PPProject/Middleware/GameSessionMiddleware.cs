using PPProject.Common;
using PPProject.Common.Session;
using System.Text.Json;

namespace PPProject.Middleware
{
    public class GameSessionMiddleware
    {
        private readonly RequestDelegate _next;
        public GameSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, RedisGameSessionStore sessionStore)
        {
            //로그인 관련은 세션 검증하지않음
            var path = context.Request.Path.Value?.ToLower();
            if (IsWhitePath(path))
            {
                await _next(context);
                return;
            }
            //Session 값이 없을 경우
            if (!TryCheckHeaderSession(context, out var userId, out var sessionId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                var errorJson = JsonSerializer.Serialize(ApiResponse<string>.Error(ErrorCode.SESSION_VALIDATION_FAILED, "Failed Session Validate : No Session Value"));
                await context.Response.WriteAsync(errorJson);
                return;
            }
            //DB내의 SessionId와 비교
            if (!await ValidateSessionAsync(sessionStore, userId, sessionId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                var errorJson = JsonSerializer.Serialize(ApiResponse<string>.Error(ErrorCode.SESSION_VALIDATION_FAILED, "Failed Session Validate : Invalid Session"));
                await context.Response.WriteAsync(errorJson);
                return;
            }
            //유저 아이디를 넘겨줌
            context.Items["uId"] = userId;
            //유저 TTL 갱신
            await sessionStore.RefreshTTL(userId);
            await _next(context);
        }

        /// <summary>
        /// 세션을 검증할 Path인지 확인
        /// </summary>
        private bool IsWhitePath(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            return path.Contains("/auth/login") || path.Contains("/auth/window");
        }

        /// <summary>
        /// 헤더내 세션값 검출 및 확인
        /// </summary>
        private bool TryCheckHeaderSession(HttpContext context, out long userId, out long sessionId)
        {
            userId = 0;
            sessionId = 0;
            //Session 값이 없을 경우
            if (!context.Request.Headers.TryGetValue("X-USER-ID", out var userIdHeader) ||
                !context.Request.Headers.TryGetValue("X-SESSION-ID", out var sessionIdHeader))
                return false;


            //Session값 형태가 long이 아닌경우
            if (!long.TryParse(userIdHeader, out userId) ||
                !long.TryParse(sessionIdHeader, out sessionId))
                return false;

            return true;
        }

        /// <summary>
        /// Redis DB내의 세션값 검증
        /// </summary>
        private async Task<bool> ValidateSessionAsync(RedisGameSessionStore sessionStore, long userId, long sessionId)
        {
            try
            {
                var dbSession = await sessionStore.GetSessionAsync(userId);
                return dbSession == sessionId;
            }
            catch (Exception ex) 
            {
                return false;
            }
        }
    }
}
