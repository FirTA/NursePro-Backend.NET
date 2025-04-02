using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NurseCounseling.API.Controllers;
using NurseCounseling.API.DTOs.Auth;
using NurseCounseling.API.Models;
using NurseCounseling.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NurseCounseling.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockTokenService = new Mock<ITokenService>();
            _mockLogger = new Mock<ILogger<AuthController>>();

            _controller = new AuthController(
                _mockUserService.Object,
                _mockTokenService.Object,
                _mockLogger.Object);

            var HttpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = HttpContext
            };
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithTokens()
        {
            var loginRequest = new LoginDto
            {
                Username = "testuser",
                Password = "password"
            };

            var user = new ApplicationUser
            {
                Id = "123",
                UserName = "testuser"
            };

            _mockUserService.Setup(x => x.AuthenticateAsync("testuser", "password"))
                .ReturnsAsync((user, "User", null));

            _mockTokenService.Setup(x => x.GenerateAccessToken(user, "User"))
                .Returns("access-token");

            _mockTokenService.Setup(x => x.GenerateRefreshTokenAsync("123"))
                .ReturnsAsync(new RefreshToken { Token = "refresh-token" });

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginResponseDto>(okResult.Value);

            Assert.Equal(123, response.UserId);
            Assert.Equal("testuser", response.Username);
            Assert.Equal("User", response.Role);
            Assert.Equal("access-token", response.AccessToken);
            Assert.Equal("refresh-token", response.RefreshToken);

            _mockUserService.Verify(x => x.RecordLoginHistoryAsync(
                "123", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

    }
}
