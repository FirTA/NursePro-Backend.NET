using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NurseCounseling.API.Data;
using NurseCounseling.API.DTOs.Auth;
using NurseCounseling.API.Models;

namespace NurseCounseling.API.Services
{

    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<(ApplicationUser User, string RoleName, string ErrorMessage)> AuthenticateAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return (null, null, "Invalid credentials");
            }

            if (user.IsLogin)
            {
                return (null, null, "User already logged in");
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                return (null, null, "Invalid credentials");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var roleName = roles.FirstOrDefault() ?? "User";

            // Mark user as logged in
            user.IsLogin = true;
            await _userManager.UpdateAsync(user);

            return (user, roleName, null);
        }

        public async Task RecordLoginHistoryAsync(string userId, string ipAddress, string deviceInfo)
        {
            var loginHistory = new LoginHistory
            {
                UserId = userId,
                LoginTime = DateTime.UtcNow,
                IpAddress = ipAddress,
                DeviceInfo = deviceInfo,
                Status = "Actice"
            };

            await _context.LoginHistories.AddAsync(loginHistory);
            await _context.SaveChangesAsync();
        }
    }
}