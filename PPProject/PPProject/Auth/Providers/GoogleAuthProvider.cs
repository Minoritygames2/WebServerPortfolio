using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace PPProject.Auth.Providers
{
    
    public static class GoogleAuthProvider
    {
        private static readonly HttpClient _httpClient = new ();

        public static async Task<(string sub, string email)> ValidateIdTokenAsync(string idToken, string clientId)
        {
            var handler = new JwtSecurityTokenHandler ();
            var jwks = await _httpClient.GetStringAsync("https://www.googleapis.com/oauth2/v3/certs");

            var keys = new JsonWebKeySet(jwks);

            var parameters = new TokenValidationParameters
            {
                ValidAudience = clientId,
                ValidIssuer = "https://accounts.google.com/",
                IssuerSigningKeys = keys.Keys
            };

            var principal = handler.ValidateToken(idToken, parameters, out var token);
            var sub = principal.FindFirst("sub")!.Value;
            var email = principal.FindFirst("email")?.Value;
            return (sub, email);
        }
    }
}
