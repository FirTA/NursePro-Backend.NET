using Microsoft.AspNetCore.Identity;

namespace NurseCounseling.API.Models
{
    public class ApplicationRole : IdentityRole
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    }
}
