using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveAllocation.Commands.DeleteLeaveAllocation;

public class DeleteLeaveAllocationCommandHandler : IRequestHandler<DeleteLeaveAllocationCommand, Unit>
{
    private readonly ILeaveAllocationRepository _leaveAllocationRepository;
    private readonly IAppLogger<DeleteLeaveAllocationCommandHandler> _logger;

    public DeleteLeaveAllocationCommandHandler(ILeaveAllocationRepository leaveAllocationRepository,        
        IAppLogger<DeleteLeaveAllocationCommandHandler> logger)
    {
        _leaveAllocationRepository = leaveAllocationRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteLeaveAllocationCommand request, CancellationToken cancellationToken)
    {
        var leaveAllocationToDelete = await _leaveAllocationRepository.GetByIdAsync(request.Id);

        if (leaveAllocationToDelete == null)
        {
            _logger.LogWarning("LeaveAllocation with id {Id} not found.", request.Id);
            throw new NotFoundException(nameof(Domain.LeaveAllocation), request.Id);
        }

        await _leaveAllocationRepository.DeleteAsync(leaveAllocationToDelete);
        _logger.LogInformation("Leave allocation {0} was successfully deleted.", request.Id);
        return Unit.Value;
    }
}
