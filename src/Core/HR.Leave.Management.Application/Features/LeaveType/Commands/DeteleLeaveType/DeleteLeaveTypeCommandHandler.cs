
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveType.Commands.DeleteLeaveType;

public class DeleteLeaveTypeCommandHandler : IRequestHandler<DeleteLeaveTypeCommand, Unit>
{
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly IAppLogger<DeleteLeaveTypeCommandHandler> _logger;

    public DeleteLeaveTypeCommandHandler(ILeaveTypeRepository leaveTypeRepository, IAppLogger<DeleteLeaveTypeCommandHandler> logger)
    {
        _leaveTypeRepository = leaveTypeRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteLeaveTypeCommand request, CancellationToken cancellationToken)
    {
               
        var leaveTypeToDelete = await _leaveTypeRepository.GetByIdAsync(request.Id);

        if (leaveTypeToDelete == null)
        {
            _logger.LogWarning("LeaveType with id {Id} not found.", request.Id);
            throw new NotFoundException(nameof(LeaveType), request.Id);
        }

        await _leaveTypeRepository.DeleteAsync(leaveTypeToDelete);
        
        _logger.LogInformation("Leave type {0} was successfully deleted.", request.Id);

        return Unit.Value;

    }
}

