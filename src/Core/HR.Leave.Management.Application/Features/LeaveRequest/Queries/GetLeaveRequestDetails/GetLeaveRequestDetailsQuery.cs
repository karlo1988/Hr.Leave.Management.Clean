using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveRequest.Queries.GetLeaveRequestDetails;

public class GetLeaveRequestDetailsQuery : IRequest<LeaveRequestDetailsDto>
{
    public int Id { get; set; }
}
