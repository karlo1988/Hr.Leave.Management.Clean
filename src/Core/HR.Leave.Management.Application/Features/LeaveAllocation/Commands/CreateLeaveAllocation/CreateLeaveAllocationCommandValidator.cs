using FluentValidation;
using HR.Leave.Management.Application.Contracts.Persistence;

namespace HR.Leave.Management.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation;

public class CreateLeaveAllocationCommandValidator : AbstractValidator<CreateLeaveAllocationCommand>
{
    private readonly ILeaveTypeRepository _leaveTypeRepository;

    public CreateLeaveAllocationCommandValidator(ILeaveTypeRepository leaveTypeRepository)
    {
        _leaveTypeRepository = leaveTypeRepository;

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

    private async Task<bool> NumberOfDaysNotExceedLeaveTypeDefault(CreateLeaveAllocationCommand command, CancellationToken cancellationToken)
    {
        var leaveType = await _leaveTypeRepository.GetByIdAsync(command.LeaveTypeId);
        return leaveType == null || command.NumberOfDays <= leaveType.DefaultDays;
    }
}
