using Microsoft.AspNetCore.Identity;

namespace NurseCounseling.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public bool IsLogin { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public virtual ICollection<LoginHistory> LoginHistories { get; set; } = new List<LoginHistory>();
    }
}
