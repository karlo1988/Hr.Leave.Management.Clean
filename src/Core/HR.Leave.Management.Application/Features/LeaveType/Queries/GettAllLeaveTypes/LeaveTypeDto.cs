
namespace HR.Leave.Management.Application.Features.LeaveType.Queries.GettAllLeaveTypes;
public class LeaveTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DefaultDays { get; set; }
}