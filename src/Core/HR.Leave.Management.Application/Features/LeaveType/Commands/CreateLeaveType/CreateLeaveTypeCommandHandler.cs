
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveType.Commands.CreateLeaveType;
public class CreateLeaveTypeCommandHandler : IRequestHandler<CreateLeaveTypeCommand, int>
{
    private readonly IMapper _mapper;
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly IAppLogger<CreateLeaveTypeCommandHandler> _logger;

    public CreateLeaveTypeCommandHandler(ILeaveTypeRepository leaveTypeRepository, IMapper mapper, IAppLogger<CreateLeaveTypeCommandHandler> logger)
    {
        _mapper = mapper;
        _leaveTypeRepository = leaveTypeRepository;
        _logger = logger;
    }

    public async Task<int> Handle(CreateLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        //Validate incoming data - if any of the rules are broken, throw an exception and stop the process
        var validator = new CreateLeaveTypeCommandValidator(_leaveTypeRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.Errors.Any())
        {
            _logger.LogWarning("Validation errors in CreateLeaveTypeCommand: {0}", validationResult.Errors);
            throw new BadRequestException("Invalid LeaveType", validationResult);
        }


        var leaveTypeToCreate = _mapper.Map<Domain.LeaveType>(request);
        await _leaveTypeRepository.AddAsync(leaveTypeToCreate);
        _logger.LogInformation("Leave type {0} was successfully created.", leaveTypeToCreate.Id);
        return leaveTypeToCreate.Id;
    }
}