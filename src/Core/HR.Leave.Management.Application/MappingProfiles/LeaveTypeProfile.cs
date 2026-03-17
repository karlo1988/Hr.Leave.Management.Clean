using AutoMapper;
using HR.Leave.Management.Application.Features.LeaveType.Queries.GettAllLeaveTypes;
using HR.Leave.Management.Application.Features.LeaveType.Queries.GetLeaveTypeDetails;
using HR.Leave.Management.Domain;
using HR.Leave.Management.Application.Features.LeaveType.Commands.CreateLeaveType;
using HR.Leave.Management.Application.Features.LeaveType.Commands.UpdateLeaveType;

namespace HR.Leave.Management.Application.MappingProfiles;

public class LeaveTypeProfile : Profile
{
    public LeaveTypeProfile()
    {
        CreateMap<LeaveTypeDto, LeaveType>().ReverseMap();
        CreateMap<GetLeaveTypeDetailsDto, LeaveType>().ReverseMap();             
        CreateMap<CreateLeaveTypeCommand, LeaveType>();    
        CreateMap<UpdateLeaveTypeCommand, LeaveType>();            
    }
}
