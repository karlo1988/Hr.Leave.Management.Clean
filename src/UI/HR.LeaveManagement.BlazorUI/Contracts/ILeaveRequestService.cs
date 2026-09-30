using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.LeaveManagement.BlazorUI.Models.LeaveRequests;
using HR.LeaveManagement.BlazorUI.Services.Base;

namespace HR.LeaveManagement.BlazorUI.Contracts
{
    public interface ILeaveRequestService
    {
        Task<List<LeaveRequestVM>> GetLeaveRequests();
        Task<LeaveRequestVM> GetLeaveRequestDetails(int id);
        Task<Response<Guid>> CreateLeaveRequest(LeaveRequestVM leaveRequest);
        Task<Response<Guid>> UpdateLeaveRequest(int id, LeaveRequestVM leaveRequest);
        Task<Response<Guid>> DeleteLeaveRequest(int id);
        Task<Response<Guid>> ChangeApproval(int id, bool approved);
    }
}
