using FluentValidation;
using HR.Leave.Management.Application.Contracts.Persistence;

namespace HR.Leave.Management.Application.Features.LeaveAllocation.Commands.UpdateLeaveAllocation;

public class UpdateLeaveAllocationCommandValidator : AbstractValidator<UpdateLeaveAllocationCommand>
{
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly ILeaveAllocationRepository _leaveAllocationRepository;

    public UpdateLeaveAllocationCommandValidator(ILeaveTypeRepository leaveTypeRepository, ILeaveAllocationRepository leaveAllocationRepository)
    {
        _leaveTypeRepository = leaveTypeRepository;
        _leaveAllocationRepository = leaveAllocationRepository;

        RuleFor(p => p.Id)
        .NotNull().WithMessage("{PropertyName} is required.")
        .MustAsync(LeaveAllocationMustExist).WithMessage("{PropertyName} does not exist.")
            .GreaterThan(0).WithMessage("{PropertyName} must be a valid allocation id.");

        RuleFor(p => p.LeaveTypeId)
            .GreaterThan(0).WithMessage("{PropertyName} must be a valid leave type.")
            .MustAsync(LeaveTypeMustExist).WithMessage("{PropertyName} does not exist.");

        RuleFor(p => p.NumberOfDays)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
            .LessThanOrEqualTo(365).WithMessage("{PropertyName} cannot exceed 365.");

        RuleFor(p => p)
            .MustAsync(NumberOfDaysNotExceedLeaveTypeDefault)
            .WithName("NumberOfDays")
            .WithMessage("{PropertyName} cannot exceed the maximum allowed days for the selected leave type.")
            .When(p => p.LeaveTypeId > 0 && p.NumberOfDays > 0);

        RuleFor(p => p.Period)
            .GreaterThan(0).WithMessage("{PropertyName} must be a valid year.");

        RuleFor(p => p.EmployeeId)
            .NotEmpty().WithMessage("{PropertyName} is required.");
    }

    private async Task<bool> LeaveTypeMustExist(int leaveTypeId, CancellationToken cancellationToken)
    {
        var leaveType = await _leaveTypeRepository.GetByIdAsync(leaveTypeId);
        return leaveType != null;
    }

    private async Task<bool> LeaveAllocationMustExist(int id, CancellationToken cancellationToken)
    {
        var leaveAllocation = await _leaveAllocationRepository.GetByIdAsync(id);
        return leaveAllocation != null;
    }

    private async Task<bool> NumberOfDaysNotExceedLeaveTypeDefault(UpdateLeaveAllocationCommand command, CancellationToken cancellationToken)
    {
        var leaveType = await _leaveTypeRepository.GetByIdAsync(command.LeaveTypeId);
        return leaveType == null || command.NumberOfDays <= leaveType.DefaultDays;
    }
}
