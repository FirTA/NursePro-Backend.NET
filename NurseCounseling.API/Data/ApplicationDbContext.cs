using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NurseCounseling.API.Models;

namespace NurseCounseling.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole,string>
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options) 
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("nursecounseling");

            builder.Entity<ApplicationUser>().ToTable("users","nursecounseling");
            builder.Entity<ApplicationRole>().ToTable("roles", "nursecounseling");
            builder.Entity<RefreshToken>().ToTable("refresh_tokens", "nursecounseling");
            builder.Entity<LoginHistory>().ToTable("login_history", "nursecounseling");

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.LoginHistories)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RefreshToken>()
                .HasIndex(t => t.Token)
                .IsUnique();

            builder.Entity<LoginHistory>()
                .HasIndex(h => h.LoginTime);

            builder.Entity<ApplicationUser>()
               .Property(u => u.FirstName)
               .IsRequired(false);

            builder.Entity<ApplicationUser>()
                .Property(u => u.LastName)
                .IsRequired(false);

            builder.Entity<ApplicationUser>()
                .Property(u => u.Phone)
                .IsRequired(false);

            builder.Entity<ApplicationUser>()
                .Property(u => u.ProfilePicture)
                .IsRequired(false);
        }

    }
}
