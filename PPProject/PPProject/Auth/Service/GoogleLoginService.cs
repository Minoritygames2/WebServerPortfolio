using Microsoft.IdentityModel.Tokens;
using PPProject.Auth.DTO;
using PPProject.Auth.Infrastructure;
using PPProject.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PPProject.Auth.Service
{
    public class GoogleLoginService : BaseLoginService
    {
        public GoogleLoginService(UserRepository userRepository) : base(userRepository)
        {
            
        }

        public override async Task<string> VerifyPlatformItemtityAsync(string platformUserId)
        {
            return await GetSubGyJWT(platformUserId);
        }

        private async Task<string> GetSubGyJWT(string idToken)
        {
            var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            var http = new HttpClient();

            var jwksJson = await http.GetStringAsync("https://www.googleapis.com/oauth2/v3/certs");
            var keys = new JsonWebKeySet(jwksJson);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuers = new List<string>
                {
                    "https://accounts.google.com",
                    "accounts.google.com"
                },
                ValidateAudience = true,
                ValidAudience = clientId,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1),

                IssuerSigningKeys = keys.Keys
            };

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(idToken, parameters, out _);

            var sub = principal.FindFirstValue("sub");

            if (string.IsNullOrEmpty(sub))
                throw new SecurityTokenException("Google Token missing sub");
            
            return sub;
        }
    }
}
