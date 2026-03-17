using HR.Leave.Management.Domain.Common;

namespace HR.Leave.Management.Domain;

public class LeaveType : BaseEntity
{    
    public string Name { get; set; } = string.Empty;
    public int DefaultDays { get; set; }

}


