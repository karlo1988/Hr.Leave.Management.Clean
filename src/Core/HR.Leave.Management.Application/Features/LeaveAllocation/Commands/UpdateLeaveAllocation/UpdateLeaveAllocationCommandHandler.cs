using AutoMapper;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveAllocation.Commands.UpdateLeaveAllocation;

public class UpdateLeaveAllocationCommandHandler : IRequestHandler<UpdateLeaveAllocationCommand, Unit>
{
    private readonly IMapper _mapper;
    private readonly ILeaveAllocationRepository _leaveAllocationRepository;
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly IAppLogger<UpdateLeaveAllocationCommandHandler> _logger;

    public UpdateLeaveAllocationCommandHandler(ILeaveAllocationRepository leaveAllocationRepository,
        ILeaveTypeRepository leaveTypeRepository, IMapper mapper, IAppLogger<UpdateLeaveAllocationCommandHandler> logger)
    {
        _mapper = mapper;
        _leaveAllocationRepository = leaveAllocationRepository;
        _leaveTypeRepository = leaveTypeRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateLeaveAllocationCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateLeaveAllocationCommandValidator(_leaveTypeRepository, _leaveAllocationRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.Errors.Any())
        {
            _logger.LogWarning("Validation errors in UpdateLeaveAllocationCommand: {0}", validationResult.Errors);
            throw new BadRequestException("Invalid LeaveAllocation", validationResult);
        }

        var existingLeaveAllocation = await _leaveAllocationRepository.GetByIdAsync(request.Id);
        if (existingLeaveAllocation == null)
        {
            _logger.LogWarning("Leave allocation with id {0} was not found.", request.Id);
            throw new NotFoundException(nameof(Domain.LeaveAllocation), request.Id);
        }

        _mapper.Map(request, existingLeaveAllocation);
        await _leaveAllocationRepository.UpdateAsync(existingLeaveAllocation);
        _logger.LogInformation("Leave allocation {0} was successfully updated.", existingLeaveAllocation.Id);
        return Unit.Value;
    }
}
