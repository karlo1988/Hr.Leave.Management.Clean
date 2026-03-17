using HR.Leave.Management.Domain.Common;

namespace HR.Leave.Management.Domain;

public class LeaveAllocation : BaseEntity
{    
    public int NumberOfDays { get; set; }
    public int LeaveTypeId { get; set; }
    public LeaveType LeaveType { get; set; } = new LeaveType();
    public int Period { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
}