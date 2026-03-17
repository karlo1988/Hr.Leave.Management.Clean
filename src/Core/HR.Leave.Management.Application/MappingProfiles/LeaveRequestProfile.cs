using AutoMapper;
using HR.Leave.Management.Application.Features.LeaveRequest.Commands.CreateLeaveRequest;
using HR.Leave.Management.Application.Features.LeaveRequest.Commands.UpdateLeaveRequest;
using HR.Leave.Management.Application.Features.LeaveRequest.Queries.GetAllLeaveRequests;
using HR.Leave.Management.Application.Features.LeaveRequest.Queries.GetLeaveRequestDetails;
using HR.Leave.Management.Domain;

namespace HR.Leave.Management.Application.MappingProfiles;

public class LeaveRequestProfile : Profile
{
    public LeaveRequestProfile()
    {
        CreateMap<LeaveRequestDto, LeaveRequest>().ReverseMap();
        CreateMap<LeaveRequest, LeaveRequestDetailsDto>();
        CreateMap<CreateLeaveRequestCommand, LeaveRequest>();
        CreateMap<UpdateLeaveRequestCommand, LeaveRequest>();
    }
}
