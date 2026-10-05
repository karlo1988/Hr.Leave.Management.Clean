using HR.Leave.Management.Application.Contracts.Identity;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveRequest.Commands.DeleteLeaveRequest;

public class DeleteLeaveRequestCommandHandler : IRequestHandler<DeleteLeaveRequestCommand, Unit>
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IAppLogger<DeleteLeaveRequestCommandHandler> _logger;
    private readonly IUserService _userService;

    public DeleteLeaveRequestCommandHandler(ILeaveRequestRepository leaveRequestRepository,
        IAppLogger<DeleteLeaveRequestCommandHandler> logger, IUserService userService)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _logger = logger;
        _userService = userService;
    }

    public async Task<Unit> Handle(DeleteLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var leaveRequestToDelete = await _leaveRequestRepository.GetByIdAsync(request.Id);

        // Employees can only delete their own requests
        if (leaveRequestToDelete == null ||
            (!_userService.IsAdministrator && leaveRequestToDelete.RequestingEmployeeId != _userService.UserId))
        {
            _logger.LogWarning("LeaveRequest with id {Id} not found.", request.Id);
            throw new NotFoundException(nameof(Domain.LeaveRequest), request.Id);
        }

        await _leaveRequestRepository.DeleteAsync(leaveRequestToDelete);
        _logger.LogInformation("Leave request {0} was successfully deleted.", request.Id);
        return Unit.Value;
    }
}
