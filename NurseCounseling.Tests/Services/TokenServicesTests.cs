using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using NurseCounseling.API.Data;
using NurseCounseling.API.Models;
using NurseCounseling.API.Services;
using NurseCounseling.API.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NurseCounseling.Tests.Services
{
    public class TokenServicesTests
    {
        private readonly JwtSettings _jwtSettings;
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;

        public TokenServicesTests()
        {
            _jwtSettings = new JwtSettings
            {
                Secret = "TestSecretKeyWithAtLeast32CharactersLong",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                AccessTokenExpireInMinutes = 15,
                RefreshTokenExpireInDays = 1
            };

            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GenerateAccessToken_ShouldReturnValidToken()
        {
            // Arrange
            var mockOptions = new Mock<IOptions<JwtSettings>>();
            mockOptions.Setup(x => x.Value).Returns(_jwtSettings);

            using var context = new ApplicationDbContext(_dbOptions);
            var tokenService = new TokenService(mockOptions.Object, context);

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testuser"
            };

            // Act
            var token = tokenService.GenerateAccessToken(user, "User");

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task GenerateRefreshToken_ShouldCreateAndSaveToken()
        {
            // Arrange
            var mockOptions = new Mock<IOptions<JwtSettings>>();
            mockOptions.Setup(x => x.Value).Returns(_jwtSettings);

            using var context = new ApplicationDbContext(_dbOptions);
            var userId = Guid.NewGuid().ToString();

            // Add a user
            context.Users.Add(new ApplicationUser {
                Id = userId,
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Phone = "123456789",
                ProfilePicture = null
            });
            await context.SaveChangesAsync();

            var tokenService = new TokenService(mockOptions.Object, context);

            // Act
            var refreshToken = await tokenService.GenerateRefreshTokenAsync(userId);

            // Assert
            Assert.NotNull(refreshToken);
            Assert.NotEmpty(refreshToken.Token);
            Assert.Equal(userId, refreshToken.UserId);
            Assert.False(refreshToken.IsRevoked);

            // Verify it was saved to the database
            var savedToken = await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken.Token);
            Assert.NotNull(savedToken);
            Assert.Equal(refreshToken.Id, savedToken.Id);
        }

        [Fact]
        public async Task RevokeRefreshToken_ShouldRevokeExistingToken()
        {
            // Arrange
            var mockOptions = new Mock<IOptions<JwtSettings>>();
            mockOptions.Setup(x => x.Value).Returns(_jwtSettings);

            using var context = new ApplicationDbContext(_dbOptions);
            var userId = Guid.NewGuid().ToString();

            // Add a user and refresh token
            context.Users.Add(new ApplicationUser {
                Id = userId,
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Phone = "123456789",
                ProfilePicture = null
            });
            var refreshToken = new RefreshToken
            {
                Token = "test-token",
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                IsRevoked = false
            };
            context.RefreshTokens.Add(refreshToken);
            await context.SaveChangesAsync();

            var tokenService = new TokenService(mockOptions.Object, context);

            // Act
            var result = await tokenService.RevokeRefreshTokenAsync("test-token");

            // Assert
            Assert.True(result);

            // Verify token was revoked in database
            var revokedToken = await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == "test-token");
            Assert.NotNull(revokedToken);
            Assert.True(revokedToken.IsRevoked);
        }

        [Fact]
        public void GetPrincipalFromExpiredToken_ShouldReturnValidPrincipal()
        {
            // Arrange
            var mockOptions = new Mock<IOptions<JwtSettings>>();
            mockOptions.Setup(x => x.Value).Returns(_jwtSettings);

            using var context = new ApplicationDbContext(_dbOptions);
            var tokenService = new TokenService(mockOptions.Object, context);

            var user = new ApplicationUser
            {
                Id = "test-user-id",
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Phone = "123456789",
                ProfilePicture = null
            };
            Console.WriteLine($"Test settings - Issuer: {_jwtSettings.Issuer}, Audience: {_jwtSettings.Audience}");
            var token = tokenService.GenerateAccessToken(user, "User");

            // Act
            var principal = tokenService.GetPrincipalFromExpiredToken(token);

            // Assert
            Assert.NotNull(principal);
            Assert.Equal("test-user-id", principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Assert.Equal("testuser", principal.FindFirst(ClaimTypes.Name)?.Value);
            Assert.Equal("User", principal.FindFirst(ClaimTypes.Role)?.Value);
        }
    }
}
