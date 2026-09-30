using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HR.LeaveManagement.BlazorUI.Models.LeaveRequests;
using HR.LeaveManagement.BlazorUI.Models.LeaveTypes;
using HR.LeaveManagement.BlazorUI.Services.Base;

namespace HR.LeaveManagement.BlazorUI.MappingProfiles
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // The generated client uses DateTimeOffset while the view models use DateTime
            CreateMap<DateTimeOffset, DateTime>().ConvertUsing(d => d.DateTime);
            CreateMap<DateTimeOffset, DateTime?>().ConvertUsing(d => d.DateTime);
            CreateMap<DateTime, DateTimeOffset>().ConvertUsing(d => new DateTimeOffset(d));
            CreateMap<DateTime?, DateTimeOffset>().ConvertUsing(d => new DateTimeOffset(d ?? default));

            CreateMap<LeaveTypeDto, LeaveTypeVM>().ReverseMap();
            CreateMap<GetLeaveTypeDetailsDto, LeaveTypeVM>().ReverseMap();
            CreateMap<CreateLeaveTypeCommand, LeaveTypeVM>().ReverseMap();
            CreateMap<UpdateLeaveTypeCommand, LeaveTypeVM>().ReverseMap();

            CreateMap<LeaveRequestDto, LeaveRequestVM>();
            CreateMap<LeaveRequestDetailsDto, LeaveRequestVM>();
            CreateMap<LeaveRequestVM, CreateLeaveRequestCommand>();
            CreateMap<LeaveRequestVM, UpdateLeaveRequestCommand>();
        }
    }
}
