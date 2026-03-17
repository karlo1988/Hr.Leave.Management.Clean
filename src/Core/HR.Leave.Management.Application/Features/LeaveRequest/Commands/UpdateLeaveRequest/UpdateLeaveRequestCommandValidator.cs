using FluentValidation;
using HR.Leave.Management.Application.Contracts.Persistence;

namespace HR.Leave.Management.Application.Features.LeaveRequest.Commands.UpdateLeaveRequest;

public class UpdateLeaveRequestCommandValidator : AbstractValidator<UpdateLeaveRequestCommand>
{
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly ILeaveRequestRepository _leaveRequestRepository;

    public UpdateLeaveRequestCommandValidator(ILeaveTypeRepository leaveTypeRepository,
        ILeaveRequestRepository leaveRequestRepository)
    {
        _leaveTypeRepository = leaveTypeRepository;
        _leaveRequestRepository = leaveRequestRepository;

        RuleFor(p => p.Id)
            .NotNull().WithMessage("{PropertyName} is required.")
            .GreaterThan(0).WithMessage("{PropertyName} must be a valid leave request id.")
            .MustAsync(LeaveRequestMustExist).WithMessage("{PropertyName} does not exist.");

        RuleFor(p => p.StartDate)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .LessThan(p => p.EndDate).WithMessage("{PropertyName} must be before the end date.");

        RuleFor(p => p.EndDate)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .GreaterThan(p => p.StartDate).WithMessage("{PropertyName} must be after the start date.");

        RuleFor(p => p.LeaveTypeId)
            .GreaterThan(0).WithMessage("{PropertyName} must be a valid leave type.")
            .MustAsync(LeaveTypeMustExist).WithMessage("{PropertyName} does not exist.");

        RuleFor(p => p.RequestComments)
            .MaximumLength(500).WithMessage("{PropertyName} must not exceed 500 characters.");

        RuleFor(p => p.RequestingEmployeeId)
            .NotEmpty().WithMessage("{PropertyName} is required.");
    }

    private async Task<bool> LeaveTypeMustExist(int leaveTypeId, CancellationToken cancellationToken)
    {
        var leaveType = await _leaveTypeRepository.GetByIdAsync(leaveTypeId);
        return leaveType != null;
    }

    private async Task<bool> LeaveRequestMustExist(int id, CancellationToken cancellationToken)
    {
        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id);
        return leaveRequest != null;
    }
}
