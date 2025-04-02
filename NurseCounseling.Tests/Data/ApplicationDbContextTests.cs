using Microsoft.EntityFrameworkCore;
using NurseCounseling.API.Data;
using NurseCounseling.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NurseCounseling.Tests.Data
{
    public class ApplicationDbContextTests
    {
        [Fact]
        public void CanCreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                  .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString())
                  .Options;

            using var context = new ApplicationDbContext(options);

            Assert.NotNull(context);
            Assert.NotNull(context.Users);
            Assert.NotNull(context.Roles);
        }

        public void CanAddAndRetrieveUser()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                  .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString())
                  .Options;

            var testUser = new ApplicationUser
            {
                UserName = "testuser",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            using (var context = new ApplicationDbContext(options))
            {
                context.Users.Add(testUser);
                context.SaveChanges();
            }

            ApplicationUser retrievedUser;
            using (var context = new ApplicationDbContext(options))
            {
                retrievedUser = context.Users.Find(testUser.Id);
            }

            Assert.NotNull(retrievedUser);
            Assert.Equal("testuser", retrievedUser.UserName);
            Assert.Equal("test@example.com", retrievedUser.Email);
            Assert.Equal("Test", retrievedUser.FirstName);
            Assert.Equal("User", retrievedUser.LastName);
        }
    }
}
