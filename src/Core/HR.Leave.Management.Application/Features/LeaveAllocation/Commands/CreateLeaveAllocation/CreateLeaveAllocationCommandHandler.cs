using HR.Leave.Management.Application.Contracts.Identity;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation;

public class CreateLeaveAllocationCommandHandler : IRequestHandler<CreateLeaveAllocationCommand, int>
{
    private readonly ILeaveAllocationRepository _leaveAllocationRepository;
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly IUserService _userService;
    private readonly IAppLogger<CreateLeaveAllocationCommandHandler> _logger;

    public CreateLeaveAllocationCommandHandler(ILeaveAllocationRepository leaveAllocationRepository,
        ILeaveTypeRepository leaveTypeRepository, IUserService userService,
        IAppLogger<CreateLeaveAllocationCommandHandler> logger)
    {
        _leaveAllocationRepository = leaveAllocationRepository;
        _leaveTypeRepository = leaveTypeRepository;
        _userService = userService;
        _logger = logger;
    }

    public async Task<int> Handle(CreateLeaveAllocationCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateLeaveAllocationCommandValidator(_leaveTypeRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.Errors.Any())
        {
            _logger.LogWarning("Validation errors in CreateLeaveAllocationCommand: {0}", validationResult.Errors);
            throw new BadRequestException("Invalid LeaveAllocation", validationResult);
        }

        var leaveType = await _leaveTypeRepository.GetByIdAsync(request.LeaveTypeId);
        var employees = await _userService.GetEmployees();
        var period = DateTime.UtcNow.Year;

        var allocations = new List<Domain.LeaveAllocation>();
        foreach (var employee in employees)
        {
            // Allocating twice must not double an employee's days
            if (await _leaveAllocationRepository.AllocationExists(employee.Id, request.LeaveTypeId, period))
                continue;

            allocations.Add(new Domain.LeaveAllocation
            {
                EmployeeId = employee.Id,
                LeaveTypeId = request.LeaveTypeId,
                NumberOfDays = leaveType.DefaultDays,
                Period = period
            });
        }

        if (allocations.Any())
            await _leaveAllocationRepository.AddAllocations(allocations);

        _logger.LogInformation("{0} leave allocations were created for leave type {1} in {2}.",
            allocations.Count, request.LeaveTypeId, period);
        return allocations.Count;
    }
}
