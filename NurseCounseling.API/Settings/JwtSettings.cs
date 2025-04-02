namespace NurseCounseling.API.Settings
{
    public class JwtSettings
    {
        public string Secret {  get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpireInMinutes { get; set; }
        public int RefreshTokenExpireInDays { get; set; }

    }
}
