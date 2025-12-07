using System.ComponentModel.DataAnnotations;

namespace PPProject.Auth.DTO.Request
{
    public class LoginRequest
    {
        [Required]
        public int PlatformCode { get; set; }
        [Required]
        public string? UserCode { get; set; }
    }
}
