namespace NurseCounseling.API.Models
{
    public class LoginHistory
    {
        public int Id { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string IpAddress { get; set; }
        public string DeviceInfo { get; set; }
        public string Status { get; set; }

        // Foreign key
        public string UserId { get; set; }

        // Navigation property
        public virtual ApplicationUser User { get; set; }

        // Helper property to calculate session duration
        public TimeSpan? SessionDuration => LogoutTime.HasValue ? LogoutTime - LoginTime : null;
    }
}
