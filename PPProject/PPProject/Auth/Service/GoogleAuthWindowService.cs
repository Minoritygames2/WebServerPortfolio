using Microsoft.AspNetCore.Session;
using PPProject.Auth.Infrastructure;
using PPProject.Common;

namespace PPProject.Auth.Service
{
    public class GoogleAuthWindowService
    {
        private readonly OAuthSessionStore _sessionStore;
        public GoogleAuthWindowService(OAuthSessionStore sessionStore)
        {
            _sessionStore = sessionStore;
        }

        /// <summary>
        /// 로그인 검증 URL 생성
        /// </summary>
        public string CreateWindowAuthUrl(string sessionId)
        {
            var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
            var redirectUri = Environment.GetEnvironmentVariable("GOOGLE_REDIRECT_UURL");
            var authUrl = "https://accounts.google.com/o/oauth2/v2/auth?"
+ "scope=openid%20profile"
+ "&response_type=code"
+ "&prompt=consent"
+ $"&state={sessionId}"
+ $"&redirect_uri={redirectUri}"
+ $"&client_id={clientId}";

            return authUrl;
        }

        /// <summary>
        /// 구글로부터 token 생성 요청
        /// </summary>
        public async Task<string> RequestOAuthTokenAsync(string code)
        {
            var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
            var redirectUri = Environment.GetEnvironmentVariable("GOOGLE_REDIRECT_UURL");


            var http = new HttpClient();
            var form = new Dictionary<string, string>()
                {
                    {"code", code },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "redirect_uri", redirectUri },
                    { "grant_type", "authorization_code" }
                };

            var result = new FormUrlEncodedContent(form);
            var tokenResponse = await http.PostAsync(
                "https://oauth2.googleapis.com/token",
                result);

            var json = await tokenResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            var idToken = json["id_token"].ToString();

            return idToken;
        }

        public async Task SaveAuthSession(string sessionId, string idToken)
        {
            var ttl = Environment.GetEnvironmentVariable("GOOGLE_SESSION_SAVE_TIME");
            if (!double.TryParse(ttl, out var ttlDouble))
                throw new Exception("ttl Parse Error");
            var ttlSpan = TimeSpan.FromSeconds(ttlDouble);
            await _sessionStore.SaveAuthSession(sessionId, idToken, string.Empty, (int)PlatformType.Google, ttlSpan);
        }

        public async Task<bool> HasAuthSession(string sessionId)
        {
            var result = await _sessionStore.IsHasKey(sessionId);
            return result;
        }

        public async Task<string> GetAuthIdToken(string sessionId)
        {
            var result = await _sessionStore.GetAuthIdToken(sessionId);
            if (string.IsNullOrEmpty(result))
                new Exception("result is null");
            return result;
        }
    }
}
