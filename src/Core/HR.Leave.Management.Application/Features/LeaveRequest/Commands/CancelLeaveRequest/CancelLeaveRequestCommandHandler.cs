using HR.Leave.Management.Application.Contracts.Email;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using HR.Leave.Management.Application.Models.Email;
using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveRequest.Commands.CancelLeaveRequest;

public class CancelLeaveRequestCommandHandler : IRequestHandler<CancelLeaveRequestCommand, Unit>
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IAppLogger<CancelLeaveRequestCommandHandler> _logger;
    private readonly IEmailSender _emailSender;

    public CancelLeaveRequestCommandHandler(ILeaveRequestRepository leaveRequestRepository,
        IAppLogger<CancelLeaveRequestCommandHandler> logger,
        IEmailSender emailSender)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _logger = logger;
        _emailSender = emailSender;
    }

    public async Task<Unit> Handle(CancelLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelLeaveRequestCommandValidator(_leaveRequestRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.Errors.Any())
        {
            _logger.LogWarning("Validation errors in CancelLeaveRequestCommand: {0}", validationResult.Errors);
            throw new BadRequestException("Invalid LeaveRequest", validationResult);
        }

        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.Id);
        if (leaveRequest == null)
        {
            _logger.LogWarning("Leave request with id {0} was not found.", request.Id);
            throw new NotFoundException(nameof(Domain.LeaveRequest), request.Id);
        }

        leaveRequest.Cancelled = true;
        await _leaveRequestRepository.UpdateAsync(leaveRequest);
        _logger.LogInformation("Leave request {0} was successfully cancelled.", request.Id);

        try
        {
            var email = new EmailMessage
            {
                To = string.Empty, // TODO: set recipient email from user profile
                Subject = "Leave Request Cancelled",
                Body = $"Your leave request from {leaveRequest.StartDate:d} to {leaveRequest.EndDate:d} has been cancelled."
            };
            await _emailSender.SendEmail(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }

        return Unit.Value;
    }
}
