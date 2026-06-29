using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HR.LeaveManagement.Identity.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(
                // admin@localhost.com → Administrator
                new IdentityUserRole<string>
                {
                    RoleId = "238e0245-76b4-45ff-999f-99baf859d76b",
                    UserId = "ec7eff12-ed15-4d11-a89c-bf0be9913144"
                },
                // john.doe@localhost.com → Employee
                new IdentityUserRole<string>
                {
                    RoleId = "389091b0-0919-4f3a-94c6-ac3d552febab",
                    UserId = "00b6c0e0-3db0-4a73-99d9-cbd0ffe50f66"
                },
                // jane.smith@localhost.com → Employee
                new IdentityUserRole<string>
                {
                    RoleId = "389091b0-0919-4f3a-94c6-ac3d552febab",
                    UserId = "3e41553c-7bad-439e-8578-060629ef3efe"
                }
            );
        }
    }
}