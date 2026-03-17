using AutoMapper;
using HR.Leave.Management.Application.Contracts.Email;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using HR.Leave.Management.Application.Models.Email;
using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveRequest.Commands.CreateLeaveRequest;

public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, int>
{
    private readonly IMapper _mapper;
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly IAppLogger<CreateLeaveRequestCommandHandler> _logger;
    private readonly IEmailSender _emailSender;

    public CreateLeaveRequestCommandHandler(IMapper mapper, ILeaveRequestRepository leaveRequestRepository,
        ILeaveTypeRepository leaveTypeRepository, IAppLogger<CreateLeaveRequestCommandHandler> logger,
        IEmailSender emailSender)
    {
        _mapper = mapper;
        _leaveRequestRepository = leaveRequestRepository;
        _leaveTypeRepository = leaveTypeRepository;
        _logger = logger;
        _emailSender = emailSender;
    }

    public async Task<int> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateLeaveRequestCommandValidator(_leaveTypeRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.Errors.Any())
        {
            _logger.LogWarning("Validation errors in CreateLeaveRequestCommand: {0}", validationResult.Errors);
            throw new BadRequestException("Invalid LeaveRequest", validationResult);
        }

        var leaveRequestToCreate = _mapper.Map<Domain.LeaveRequest>(request);
        leaveRequestToCreate.DateRequested = DateTime.UtcNow;

        await _leaveRequestRepository.AddAsync(leaveRequestToCreate);
        _logger.LogInformation("Leave request {0} was successfully created.", leaveRequestToCreate.Id);

        try
        {

            var email = new EmailMessage
            {
                To = string.Empty, // TODO: set recipient email from user profile
                Subject = "Leave Request Submitted",
                Body = $"Your leave request from {request.StartDate:d} to {request.EndDate:d} has been submitted successfully."
            };
            await _emailSender.SendEmail(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);

        }

        return leaveRequestToCreate.Id;
    }
}
