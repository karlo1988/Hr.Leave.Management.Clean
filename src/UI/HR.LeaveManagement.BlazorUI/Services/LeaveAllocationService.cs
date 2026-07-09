using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Services.Base;
using Blazored.LocalStorage;

namespace HR.LeaveManagement.BlazorUI.Services
{
    public class LeaveAllocationService: BaseHttpService, ILeaveAllocationService
    {
        public LeaveAllocationService(IClient client, ILocalStorageService localStorage) : base(client, localStorage)
        {
            
        }
    }
}