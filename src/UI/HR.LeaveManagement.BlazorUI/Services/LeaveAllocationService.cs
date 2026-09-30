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

        public async Task<Response<Guid>> CreateLeaveAllocations(int leaveTypeId)
        {
            try
            {
                var command = new CreateLeaveAllocationCommand { LeaveTypeId = leaveTypeId };
                await AddBearerToken();
                await _client.LeaveAllocationsPOSTAsync(command);
                return new Response<Guid>() { Success = true };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<Guid>(ex);
            }
        }
    }
}
