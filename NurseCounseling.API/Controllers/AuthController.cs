using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NurseCounseling.API.DTOs.Auth;
using NurseCounseling.API.Services;

namespace NurseCounseling.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ITokenService tokenService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Login attempt for user : {username}", loginRequest.Username);

            var (user, roleName, errorMessage) = await _userService.AuthenticateAsync(loginRequest.Username, loginRequest.Password);


            if (user == null)
            {
                _logger.LogWarning("Failed login attempt for user: {Username}. Reason: {Reason}",
                    loginRequest.Username, errorMessage);
                return Unauthorized(new { message = errorMessage });
            }

            var accessToken = _tokenService.GenerateAccessToken(user, roleName);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

            await _userService.RecordLoginHistoryAsync(
                user.Id,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers["User-Agent"].ToString());

            _logger.LogInformation("Successful login for user: {Username} with role: {Role}",
                           user.UserName, roleName);

            return Ok(new LoginResponseDto
            {
                UserId = int.Parse(user.Id),
                Username = user.UserName,
                Role = roleName,
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            });
        }

    }
}
