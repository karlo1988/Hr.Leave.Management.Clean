using AutoMapper;
using HR.Leave.Management.Application.Contracts.Identity;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveRequest.Queries.GetAllLeaveRequests;

public class GetLeaveRequestsQueryHandler : IRequestHandler<GetLeaveRequestsQuery, List<LeaveRequestDto>>
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetLeaveRequestsQueryHandler> _logger;
    private readonly IUserService _userService;

    public GetLeaveRequestsQueryHandler(ILeaveRequestRepository leaveRequestRepository, IMapper mapper,
        IAppLogger<GetLeaveRequestsQueryHandler> logger, IUserService userService)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _mapper = mapper;
        _logger = logger;
        _userService = userService;
    }

    public async Task<List<LeaveRequestDto>> Handle(GetLeaveRequestsQuery request, CancellationToken cancellationToken)
    {
        // Administrators see every request, employees only their own
        var leaveRequests = _userService.IsAdministrator
            ? await _leaveRequestRepository.GetLeaveRequestsWithDetails()
            : await _leaveRequestRepository.GetLeaveRequestsWithDetails(_userService.UserId);
        var requests = _mapper.Map<List<LeaveRequestDto>>(leaveRequests);
        _logger.LogInformation("Leave requests were successfully retrieved from the database.");
        return requests;
    }
}
