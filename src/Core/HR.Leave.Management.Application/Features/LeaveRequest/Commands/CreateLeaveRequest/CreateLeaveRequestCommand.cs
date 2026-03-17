using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveRequest.Commands.CreateLeaveRequest;

public class CreateLeaveRequestCommand : IRequest<int>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int LeaveTypeId { get; set; }
    public string RequestComments { get; set; } = string.Empty;
    public string RequestingEmployeeId { get; set; } = string.Empty;
}
