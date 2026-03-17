
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveType.Commands.UpdateLeaveType;
public class UpdateLeaveTypeCommandHandler : IRequestHandler<UpdateLeaveTypeCommand, Unit>
{
    private readonly IMapper _mapper;
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly IAppLogger<UpdateLeaveTypeCommandHandler> _logger;


    public UpdateLeaveTypeCommandHandler(IMapper mapper, ILeaveTypeRepository leaveTypeRepository, IAppLogger<UpdateLeaveTypeCommandHandler> logger)
    {
        _mapper = mapper;
        _leaveTypeRepository = leaveTypeRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateLeaveTypeCommand request, CancellationToken cancellationToken)
    {        
        var validator = new UpdateLeaveTypeCommandValidator(_leaveTypeRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (validationResult.Errors.Count > 0)
        {
             _logger.LogWarning("Validation errors in UpdateLeaveTypeCommand: {0}", validationResult.Errors);
            throw new BadRequestException("Invalid LeaveType", validationResult);
        }

        var leaveTypeToUpdate = await _leaveTypeRepository.GetByIdAsync(request.Id);
        if (leaveTypeToUpdate == null)
        {
             _logger.LogWarning("Leave type with id {0} was not found.", request.Id);
            throw new NotFoundException(nameof(Domain.LeaveType), request.Id);
        }
    
        _mapper.Map(request, leaveTypeToUpdate);
        await _leaveTypeRepository.UpdateAsync(leaveTypeToUpdate);        
         _logger.LogInformation("Leave type {0} was successfully updated.", leaveTypeToUpdate.Id);
        return Unit.Value;
    }
}