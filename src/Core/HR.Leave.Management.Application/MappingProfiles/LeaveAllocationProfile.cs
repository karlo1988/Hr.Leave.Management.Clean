using AutoMapper;
using HR.Leave.Management.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation;
using HR.Leave.Management.Application.Features.LeaveAllocation.Commands.UpdateLeaveAllocation;
using HR.Leave.Management.Application.Features.LeaveAllocation.Queries.GetAllLeaveAllocations;
using HR.Leave.Management.Application.Features.LeaveAllocation.Queries.GetLeaveAllocationDetails;
using HR.Leave.Management.Domain;

namespace HR.Leave.Management.Application.MappingProfiles;

public class LeaveAllocationProfile : Profile
{
    public LeaveAllocationProfile()
    {
        CreateMap<LeaveAllocationDto, LeaveAllocation>().ReverseMap();
        CreateMap<LeaveAllocation, LeaveAllocationDetailsDto>()
            .ForMember(dest => dest.LeaveTypeName, opt => opt.MapFrom(src => src.LeaveType.Name));
        CreateMap<CreateLeaveAllocationCommand, LeaveAllocation>();
        CreateMap<UpdateLeaveAllocationCommand, LeaveAllocation>();
    }
}
