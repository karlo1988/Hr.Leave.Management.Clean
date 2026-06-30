using HR.LeaveManagement.Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace HR.LeaveManagement.Identity.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ApplicationUser> builder)
        {
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
                    PasswordHash = "AQAAAAIAAYagAAAAEDAPBazKxc/ro5Tj7kqTQaMskAAUMv4PT2Bn+Lu0tbd/5PSMre2axWPhiCOLgp7kbA==",
                    EmailConfirmed = true,
                    SecurityStamp = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                    ConcurrencyStamp = "a1b2c3d4-e5f6-7890-abcd-ef1234567891"
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
                    PasswordHash = "AQAAAAIAAYagAAAAEICbmZyvU0eRhWd6JLqb0/XER3OsSKN/Qri/eVWRfsJvGV5eSI3UcpvrxeU5BCKzBg==",
                    EmailConfirmed = true,
                    SecurityStamp = "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                    ConcurrencyStamp = "b2c3d4e5-f6a7-8901-bcde-f12345678902"
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
                    PasswordHash = "AQAAAAIAAYagAAAAECWgdMFEwZRh5VV0TGmZ2vDeE2UmciQieURubqlFwzHpkfwPHx9xdtdtEA6+gLIt3g==",
                    EmailConfirmed = true,
                    SecurityStamp = "c3d4e5f6-a7b8-9012-cdef-123456789012",
                    ConcurrencyStamp = "c3d4e5f6-a7b8-9012-cdef-123456789013"
                }
            );
        }
    }
}