using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveType.Commands.DeleteLeaveType;
public class DeleteLeaveTypeCommand: IRequest<Unit>
{
    public int Id { get; set; }
}