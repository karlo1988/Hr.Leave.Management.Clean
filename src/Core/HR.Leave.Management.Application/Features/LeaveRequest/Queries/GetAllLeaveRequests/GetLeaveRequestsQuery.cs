using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveRequest.Queries.GetAllLeaveRequests;

public class GetLeaveRequestsQuery : IRequest<List<LeaveRequestDto>>
{
}
