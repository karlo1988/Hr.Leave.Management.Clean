using AutoMapper;
using HR.Leave.Management.Application.Contracts.Identity;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using MediatR;

namespace HR.Leave.Management.Application.Features.LeaveRequest.Queries.GetLeaveRequestDetails;

public class GetLeaveRequestDetailsQueryHandler : IRequestHandler<GetLeaveRequestDetailsQuery, LeaveRequestDetailsDto>
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public GetLeaveRequestDetailsQueryHandler(ILeaveRequestRepository leaveRequestRepository, IMapper mapper,
        IUserService userService)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<LeaveRequestDetailsDto> Handle(GetLeaveRequestDetailsQuery request, CancellationToken cancellationToken)
    {
        var leaveRequest = await _leaveRequestRepository.GetLeaveRequestWithDetails(request.Id);

        // Someone else's request is reported as not found, so employees cannot probe for ids
        if (leaveRequest == null ||
            (!_userService.IsAdministrator && leaveRequest.RequestingEmployeeId != _userService.UserId))
            throw new NotFoundException(nameof(Domain.LeaveRequest), request.Id);

        return _mapper.Map<LeaveRequestDetailsDto>(leaveRequest);
    }
}
