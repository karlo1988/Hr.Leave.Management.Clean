using HR.Leave.Management.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Leave.Management.Persistence.Configurations;

public class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> builder)
    {
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        builder.HasData(
            new LeaveType { Id = 1, Name = "Vacation", DefaultDays = 10, CreatedDate = seedDate },
            new LeaveType { Id = 2, Name = "Sick", DefaultDays = 12, CreatedDate = seedDate },
            new LeaveType { Id = 3, Name = "Maternity", DefaultDays = 90, CreatedDate = seedDate }
        );

        builder.Property(q => q.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}