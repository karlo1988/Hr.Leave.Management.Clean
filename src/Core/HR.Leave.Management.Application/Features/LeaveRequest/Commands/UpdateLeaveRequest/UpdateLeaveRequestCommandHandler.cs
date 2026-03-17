using AutoMapper;
using HR.Leave.Management.Application.Contracts.Email;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using HR.Leave.Management.Application.Models.Email;
using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveRequest.Commands.UpdateLeaveRequest;

public class UpdateLeaveRequestCommandHandler : IRequestHandler<UpdateLeaveRequestCommand, Unit>
{
    private readonly IMapper _mapper;
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly IAppLogger<UpdateLeaveRequestCommandHandler> _logger;
    private readonly IEmailSender _emailSender;

    public UpdateLeaveRequestCommandHandler(IMapper mapper, ILeaveRequestRepository leaveRequestRepository,
        ILeaveTypeRepository leaveTypeRepository, IAppLogger<UpdateLeaveRequestCommandHandler> logger,
        IEmailSender emailSender)
    {
        _mapper = mapper;
        _leaveRequestRepository = leaveRequestRepository;
        _leaveTypeRepository = leaveTypeRepository;
        _logger = logger;
        _emailSender = emailSender;
    }

    public async Task<Unit> Handle(UpdateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateLeaveRequestCommandValidator(_leaveTypeRepository, _leaveRequestRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.Errors.Any())
        {
            _logger.LogWarning("Validation errors in UpdateLeaveRequestCommand: {0}", validationResult.Errors);
            throw new BadRequestException("Invalid LeaveRequest", validationResult);
        }

        var existingLeaveRequest = await _leaveRequestRepository.GetByIdAsync(request.Id);
        if (existingLeaveRequest == null)
        {
            _logger.LogWarning("Leave request with id {0} was not found.", request.Id);
            throw new NotFoundException(nameof(Domain.LeaveRequest), request.Id);
        }

        _mapper.Map(request, existingLeaveRequest);
        await _leaveRequestRepository.UpdateAsync(existingLeaveRequest);
        _logger.LogInformation("Leave request {0} was successfully updated.", existingLeaveRequest.Id);

        try
        {
            var email = new EmailMessage
            {
                To = string.Empty, // TODO: set recipient email from user profile
                Subject = "Leave Request Updated",
                Body = $"Your leave request for {existingLeaveRequest.StartDate:d} to {existingLeaveRequest.EndDate:d} has been updated successfully."
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
