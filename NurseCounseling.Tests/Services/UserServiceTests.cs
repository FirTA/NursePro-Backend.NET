using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NurseCounseling.API.Data;
using NurseCounseling.API.DTOs.Auth;
using NurseCounseling.API.Models;
using NurseCounseling.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NurseCounseling.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<RoleManager<ApplicationRole>> _mockRoleManager;
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;


        public UserServiceTests() 
        {
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString())
                .Options;

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            var RoleStoreMock = new Mock<IRoleStore<ApplicationRole>>();
            _mockRoleManager = new Mock<RoleManager<ApplicationRole>>(
                RoleStoreMock.Object, null, null, null, null);

        }

        [Fact]
        public async Task AutenticaAuthenticateAsync_WithValidCredentials_ReturnsUserAndRole()
        {
            var testUser = new ApplicationUser
            {
                Id = "test-user-id",
                UserName = "testuser",
                IsLogin = false,
                FirstName = "Test",
                LastName = "User",
                Phone = "1234567890"
            };

            _mockUserManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(testUser);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(testUser, "password"))
                          .ReturnsAsync(true);
            _mockUserManager.Setup(x => x.GetRolesAsync(testUser))
                .ReturnsAsync(new List<string> { "User" });
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            using var context = new ApplicationDbContext(_dbOptions);
            var service = new UserService(_mockUserManager.Object, _mockRoleManager.Object, context);

            var result = await service.AuthenticateAsync("testuser", "password");

            Assert.NotNull(result.User);
            Assert.Equal("User", result.RoleName);
            Assert.Null(result.ErrorMessage);
            Assert.True(result.User.IsLogin);

        }

        [Fact]
        public async Task AuthenticateAsync_WithInvalidCredentials_ReturnsError()
        {
            var testUser = new ApplicationUser
            {
                Id = "test-user-id",
                UserName = "testuser",
                IsLogin = false,
            };

            _mockUserManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(testUser);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(testUser, "wrong-password"))
                          .ReturnsAsync(false);

            using var context = new ApplicationDbContext(_dbOptions);
            var service = new UserService(_mockUserManager.Object, _mockRoleManager.Object, context);

            var result = await service.AuthenticateAsync("testuser", "wrong-password");

            Assert.Null(result.User);
            Assert.Null(result.RoleName);
            Assert.Equal("Invalid credentials", result.ErrorMessage);

        }

        [Fact]
        public async Task AuthenticateAsync_WhenUserAlreadyLoggedIn_ReturnsError()
        {
            // Arrange
            var testUser = new ApplicationUser
            {
                Id = "test-user-id",
                UserName = "testuser",
                IsLogin = true // User is already logged in
            };

            _mockUserManager.Setup(x => x.FindByNameAsync("testuser"))
                .ReturnsAsync(testUser);

            using var context = new ApplicationDbContext(_dbOptions);
            var service = new UserService(_mockUserManager.Object, _mockRoleManager.Object, context);

            // Act
            var result = await service.AuthenticateAsync("testuser", "password");

            // Assert
            Assert.Null(result.User);
            Assert.Null(result.RoleName);
            Assert.Equal("User already logged in", result.ErrorMessage);
        }

    }

}
