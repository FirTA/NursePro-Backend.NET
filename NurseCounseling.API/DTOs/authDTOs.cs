using System.ComponentModel.DataAnnotations;

namespace NurseCounseling.API.DTOs
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

    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }

    public class RegistrationRequestDto
    {
        [Required]
        [MinLength(5)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

    }

    public class ChangePasswordRequestDto
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }

    }

    public class ResetPasswordConfirmRequestDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }

    }

    public class UserProfileDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string ProfilePicture { get; set; } // Base64 encoded

    }

    public class NurseProfileDto
    {
        public int Id { get; set; }
        public string NurseAccountId { get; set; }
        public string Level { get; set; }
        public string Department { get; set; }

        public NurseProfileDto Nurse {  get; set; }
        public ManagementProfileDto Management { get; set; }

    }

    public class ManagementProfileDto
    {
        public int Id { get; set; }
        public string ManagementAccountId { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
    }
}
