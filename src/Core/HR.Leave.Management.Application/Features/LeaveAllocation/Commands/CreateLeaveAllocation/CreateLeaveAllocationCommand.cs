using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation;

/// <summary>
/// Allocates the leave type's default number of days to every employee for the current year.
/// Returns the number of allocations created.
/// </summary>
public class CreateLeaveAllocationCommand : IRequest<int>
{
    public int LeaveTypeId { get; set; }
}
