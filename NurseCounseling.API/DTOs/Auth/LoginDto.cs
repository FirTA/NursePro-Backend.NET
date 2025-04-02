using System.ComponentModel.DataAnnotations;

namespace NurseCounseling.API.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required]
        [MinLength(5)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
