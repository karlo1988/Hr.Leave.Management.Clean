using HR.Leave.Management.Application.Contracts.Email;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using HR.Leave.Management.Application.Models.Email;
using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveRequest.Commands.ChangeLeaveRequestApproval;

public class ChangeLeaveRequestApprovalCommandHandler : IRequestHandler<ChangeLeaveRequestApprovalCommand, Unit>
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IAppLogger<ChangeLeaveRequestApprovalCommandHandler> _logger;
    private readonly IEmailSender _emailSender;

    public ChangeLeaveRequestApprovalCommandHandler(ILeaveRequestRepository leaveRequestRepository,
        IAppLogger<ChangeLeaveRequestApprovalCommandHandler> logger, IEmailSender emailSender)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _logger = logger;
        _emailSender = emailSender;
    }

    public async Task<Unit> Handle(ChangeLeaveRequestApprovalCommand request, CancellationToken cancellationToken)
    {
        var validator = new ChangeLeaveRequestApprovalCommandValidator(_leaveRequestRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.Errors.Any())
        {
            _logger.LogWarning("Validation errors in ChangeLeaveRequestApprovalCommand: {0}", validationResult.Errors);
            throw new BadRequestException("Invalid LeaveRequest approval", validationResult);
        }

        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.Id);
        if (leaveRequest == null)
        {
            _logger.LogWarning("Leave request with id {0} was not found.", request.Id);
            throw new NotFoundException(nameof(Domain.LeaveRequest), request.Id);
        }

        leaveRequest.Approved = request.Approved;
        await _leaveRequestRepository.UpdateAsync(leaveRequest);
        try
        {
            var email = new EmailMessage
            {
                To = string.Empty, // TODO: set recipient email from user profile
                Subject = "Leave Request Approval Status Changed",
                Body = $"Your leave request from {leaveRequest.StartDate:d} to {leaveRequest.EndDate:d} has been {(request.Approved == true ? "approved" : "rejected")}."
            };
            await _emailSender.SendEmail(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }



        _logger.LogInformation("Leave request {0} approval status was changed to {1}.", request.Id, request.Approved);
        return Unit.Value;
    }
}
