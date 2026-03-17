namespace HR.Leave.Management.Application.Features.LeaveType.Queries.GetLeaveTypeDetails;
public class GetLeaveTypeDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DefaultDays { get; set; }
    public DateTime CreatedDate { get; set; } 
    public DateTime? ModifiedDate { get; set; }
}