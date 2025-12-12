namespace PPProject.Auth.DTO
{
    public class OAuthInfo
    {
        public string? IdToken { get; set; }
        public string? AuthCode { get; set; }
        public int Platform { get; set; }
    }
}
