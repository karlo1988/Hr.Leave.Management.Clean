using FluentValidation;
using HR.Leave.Management.Application.Contracts.Persistence;

namespace HR.Leave.Management.Application.Features.LeaveType.Commands.CreateLeaveType;

public class CreateLeaveTypeCommandValidator : AbstractValidator<CreateLeaveTypeCommand>
{
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    public CreateLeaveTypeCommandValidator(ILeaveTypeRepository leaveTypeRepository)
    {
        _leaveTypeRepository = leaveTypeRepository;

        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull()
            .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

        RuleFor(p => p.DefaultDays)
            .GreaterThan(0).WithMessage("{PropertyName} cannot be less than 1.")
            .LessThanOrEqualTo(100).WithMessage("{PropertyName} cannot exceed 100.");

        RuleFor(p => p).MustAsync(LeaveTypeNameUnique).WithMessage("Leave Type with the same name already exists.");
    }

    private async Task<bool> LeaveTypeNameUnique(CreateLeaveTypeCommand command, CancellationToken cancellationToken)
    {
        return await _leaveTypeRepository.IsLeaveTypeNameUnique(command.Name);           
    }
}