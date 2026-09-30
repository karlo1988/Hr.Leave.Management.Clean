using HR.Leave.Management.Domain.Common;

namespace HR.Leave.Management.Domain;

public class LeaveAllocation : BaseEntity
{    
    public int NumberOfDays { get; set; }
    public int LeaveTypeId { get; set; }
    // Left null unless loaded: a new LeaveType here would be inserted by EF as a new, empty leave type
    public LeaveType? LeaveType { get; set; }
    public int Period { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
}