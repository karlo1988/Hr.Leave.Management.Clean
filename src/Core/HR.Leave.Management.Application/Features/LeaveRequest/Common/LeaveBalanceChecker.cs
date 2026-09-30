using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;

namespace HR.Leave.Management.Application.Features.LeaveRequest.Common;

/// <summary>
/// Checks a leave request against the employee's allocation.
/// An allocation's NumberOfDays is the remaining balance: approved requests are deducted from it,
/// and pending requests are held against it until they are approved or rejected.
/// </summary>
public class LeaveBalanceChecker
{
    private readonly ILeaveAllocationRepository _leaveAllocationRepository;
    private readonly ILeaveRequestRepository _leaveRequestRepository;

    public LeaveBalanceChecker(ILeaveAllocationRepository leaveAllocationRepository,
        ILeaveRequestRepository leaveRequestRepository)
    {
        _leaveAllocationRepository = leaveAllocationRepository;
        _leaveRequestRepository = leaveRequestRepository;
    }

    // Both the first and the last day count as leave, so 5–9 October is 5 days
    public static int CountDays(DateTime startDate, DateTime endDate) => (endDate.Date - startDate.Date).Days + 1;

    public async Task EnsureEnoughDays(string employeeId, int leaveTypeId, DateTime startDate, DateTime endDate,
        int? excludedLeaveRequestId = null)
    {
        var period = startDate.Year;
        var allocation = await _leaveAllocationRepository.GetUserAllocations(employeeId, leaveTypeId, period);
        if (allocation == null)
            throw new BadRequestException($"You do not have any allocations for this leave type in {period}.");

        var employeeRequests = await _leaveRequestRepository.GetLeaveRequestsWithDetails(employeeId);
        var pendingDays = employeeRequests
            .Where(r => r.LeaveTypeId == leaveTypeId
                        && r.StartDate.Year == period
                        && r.Approved == null
                        && !r.Cancelled
                        && r.Id != excludedLeaveRequestId)
            .Sum(r => CountDays(r.StartDate, r.EndDate));

        var availableDays = allocation.NumberOfDays - pendingDays;
        var requestedDays = CountDays(startDate, endDate);
        if (requestedDays > availableDays)
            throw new BadRequestException(
                $"You do not have enough days for this request: requested {requestedDays}, available {Math.Max(availableDays, 0)}.");
    }
}
