using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using HR.Leave.Management.Application.Contracts.Persistence;

namespace HR.Leave.Management.Application.Features.LeaveType.Commands.UpdateLeaveType
{
    public class UpdateLeaveTypeCommandValidator : AbstractValidator<UpdateLeaveTypeCommand>
    {
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        public UpdateLeaveTypeCommandValidator(ILeaveTypeRepository leaveTypeRepository)
        {
            _leaveTypeRepository = leaveTypeRepository;

            RuleFor(p => p.Id)
            .NotNull().WithMessage("{PropertyName} is required.")
            .MustAsync(LeaveTypeExists).WithMessage("Leave Type does not exist.");              


            RuleFor(p => p.Name)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull()
            .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

            RuleFor(p => p.DefaultDays)
            .LessThanOrEqualTo(100).WithMessage("{PropertyName} cannot exceed 100.")
            .GreaterThan(0).WithMessage("{PropertyName} cannot be less than 1.");

            RuleFor(p => p).MustAsync(LeaveTypeNameUnique).WithMessage("Leave Type with the same name already exists.");
        

        }

        private async Task<bool> LeaveTypeExists(int id, CancellationToken cancellationToken)
        {
            return await _leaveTypeRepository.LeaveTypeExists(id);           
        }

        private async Task<bool> LeaveTypeNameUnique(UpdateLeaveTypeCommand command, CancellationToken cancellationToken)
        {
            return await _leaveTypeRepository.IsLeaveTypeNameUnique(command.Name);           
        }
    }
}