using HR.Leave.Management.Application.Features.LeaveType.Queries.GettAllLeaveTypes;

namespace HR.Leave.Management.Application.Features.LeaveAllocation.Queries.GetAllLeaveAllocations;

public class LeaveAllocationDto
{
    public int Id { get; set; }
    public int NumberOfDays { get; set; }
    public int LeaveTypeId { get; set; }
    public LeaveTypeDto LeaveType { get; set; } 
    public int Period { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
}
