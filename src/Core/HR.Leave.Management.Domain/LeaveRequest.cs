

using HR.Leave.Management.Domain.Common;

namespace HR.Leave.Management.Domain;

public class LeaveRequest: BaseEntity
{    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int LeaveTypeId { get; set; }
    // Left null unless loaded: a new LeaveType here would be inserted by EF as a new, empty leave type
    public LeaveType? LeaveType { get; set; }
    public DateTime DateRequested { get; set; }
    public string RequestComments { get; set; } = string.Empty;
    public bool? Approved { get; set; }
    public bool Cancelled { get; set; }
    public string RequestingEmployeeId { get; set; } = string.Empty;
}