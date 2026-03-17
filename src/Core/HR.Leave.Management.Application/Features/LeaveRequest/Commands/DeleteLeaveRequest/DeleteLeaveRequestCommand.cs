using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveRequest.Commands.DeleteLeaveRequest;

public class DeleteLeaveRequestCommand : IRequest<Unit>
{
    public int Id { get; set; }
}
