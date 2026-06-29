using HR.LeaveManagement.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HR.LeaveManagement.Identity.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ApplicationUser> builder)
        {

            var hasher = new PasswordHasher<ApplicationUser>();
            builder.HasData(
                new ApplicationUser
                {
                    Id = "ec7eff12-ed15-4d11-a89c-bf0be9913144",
                    Email = "admin@localhost.com",
                    NormalizedEmail = "ADMIN@LOCALHOST.COM",
                    FirstName = "System",
                    LastName = "Admin",
                    UserName = "admin@localhost.com",
                    NormalizedUserName = "ADMIN@LOCALHOST.COM",
                    PasswordHash = hasher.HashPassword(null, "AdminPassword123!"),
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    Id = "00b6c0e0-3db0-4a73-99d9-cbd0ffe50f66",
                    Email = "john.doe@localhost.com",
                    NormalizedEmail = "JOHN.DOE@LOCALHOST.COM",
                    FirstName = "John",
                    LastName = "Doe",
                    UserName = "john.doe@localhost.com",
                    NormalizedUserName = "JOHN.DOE@LOCALHOST.COM",
                    PasswordHash = hasher.HashPassword(null, "JohnDoePassword123!"),
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    Id = "3e41553c-7bad-439e-8578-060629ef3efe",
                    Email = "jane.smith@localhost.com",
                    NormalizedEmail = "JANE.SMITH@LOCALHOST.COM",
                    FirstName = "Jane",
                    LastName = "Smith",
                    UserName = "jane.smith@localhost.com",
                    NormalizedUserName = "JANE.SMITH@LOCALHOST.COM",
                    PasswordHash = hasher.HashPassword(null, "JaneSmithPassword123!"),
                    EmailConfirmed = true
                }
            );
        }
    }
}