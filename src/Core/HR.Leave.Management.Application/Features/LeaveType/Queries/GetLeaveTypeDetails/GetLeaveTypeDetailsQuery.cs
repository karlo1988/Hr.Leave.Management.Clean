using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveType.Queries.GetLeaveTypeDetails;
public class GetLeaveTypeDetailsQuery: IRequest<GetLeaveTypeDetailsDto>
{
    public int Id { get; set; }
}