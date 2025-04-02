using NurseCounseling.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NurseCounseling.Tests.Models
{
    public class ApplicationUserTests
    {
        [Fact]
        public void ApplicationUser_CreatedAt_ShouldSetDefaultValue()
        {
            var user = new ApplicationUser();
            
            Assert.NotEqual(default, user.CreatedAt);
            Assert.True(DateTime.UtcNow.AddMinutes(-1) <= user.CreatedAt);
            Assert.True(user.CreatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void ApplicationUser_UpdatedAt_ShouldSetDefaultValue()
        {
            var user = new ApplicationUser();

            Assert.NotEqual(default, user.UpdatedAt);
            Assert.True(DateTime.UtcNow.AddMinutes(-1) <= user.UpdatedAt);
            Assert.True(user.UpdatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void ApplicationUser_IsLogin_ShouldDefaultToFalse()
        {
            var user = new ApplicationUser();

            Assert.False(user.IsLogin);
        }
    }
}
