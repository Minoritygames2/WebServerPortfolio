namespace PPProject.Auth.DTO.Response
{
    public class LoginResponse
    {
        public long Session { get; set; }
        public long UserId { get; set; }
        public int Status { get; set; }
    }
}
