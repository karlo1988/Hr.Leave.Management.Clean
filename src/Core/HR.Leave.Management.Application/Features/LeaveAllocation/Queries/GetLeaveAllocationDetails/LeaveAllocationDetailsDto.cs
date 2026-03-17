using HR.Leave.Management.Application.Features.LeaveType.Queries.GettAllLeaveTypes;

namespace HR.Leave.Management.Application.Features.LeaveAllocation.Queries.GetLeaveAllocationDetails;

public class LeaveAllocationDetailsDto
{
    public int Id { get; set; }
    public int NumberOfDays { get; set; }
    public LeaveTypeDto LeaveType { get; set; }
    public int LeaveTypeId { get; set; }    
    public string LeaveTypeName { get; set; } = string.Empty;
    public int Period { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}
