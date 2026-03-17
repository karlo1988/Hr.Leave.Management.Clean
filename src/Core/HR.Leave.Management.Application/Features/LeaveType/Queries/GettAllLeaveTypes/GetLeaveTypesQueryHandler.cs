
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveType.Queries.GettAllLeaveTypes;
public class GetLeaveTypesQueryHandler : IRequestHandler<GetLeaveTypesQuery, List<LeaveTypeDto>>
{
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetLeaveTypesQueryHandler> _logger;

    public GetLeaveTypesQueryHandler(ILeaveTypeRepository leaveTypeRepository, IMapper mapper, IAppLogger<GetLeaveTypesQueryHandler> logger)
    {
        _leaveTypeRepository = leaveTypeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<LeaveTypeDto>> Handle(GetLeaveTypesQuery request, CancellationToken cancellationToken)
    {
        //Query the database to get all leave types
        var leaveTypes = await _leaveTypeRepository.GetAllAsync();

        //convert to list of leave type dtos and return
        var data = _mapper.Map<List<LeaveTypeDto>>(leaveTypes);
        //log that the leave types were successfully retrieved from the database
        _logger.LogInformation("Leave types were successfully retrieved from the database.");

        //return the list of leave type dtos
        return data;
    }
}