using NurseCounseling.API.DTOs.Auth;
using NurseCounseling.API.Models;

namespace NurseCounseling.API.Services
{
    public interface IUserService
    {
        Task<(ApplicationUser User,string RoleName, string ErrorMessage)> AuthenticateAsync(string username, string password);
        Task RecordLoginHistoryAsync(string userId, string ipAddress, string deviceInfo);
    }
}
