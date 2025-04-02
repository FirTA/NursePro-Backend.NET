namespace NurseCounseling.API.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key
        public string UserId { get; set; }

        // Navigation property
        public virtual ApplicationUser User { get; set; }
        public bool IsActive => !IsRevoked && ExpiryDate > DateTime.UtcNow;
    }
}
