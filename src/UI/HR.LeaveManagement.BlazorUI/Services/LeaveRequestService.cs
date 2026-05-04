using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Services.Base;

namespace HR.LeaveManagement.BlazorUI.Services
{
    public class LeaveRequestService: BaseHttpService, ILeaveRequestService
    {
        public LeaveRequestService(IClient client) : base(client)
        {
            
        }
    }
}