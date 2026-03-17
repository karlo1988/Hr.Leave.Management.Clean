using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveAllocation.Queries.GetAllLeaveAllocations;

public class GetLeaveAllocationsQuery : IRequest<List<LeaveAllocationDto>>
{
}
