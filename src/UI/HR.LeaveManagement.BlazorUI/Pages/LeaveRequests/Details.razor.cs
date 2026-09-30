using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models.LeaveRequests;
using Microsoft.AspNetCore.Components;

namespace HR.LeaveManagement.BlazorUI.Pages.LeaveRequests
{
    public partial class Details
    {
        [Inject]
        private ILeaveRequestService LeaveRequestService { get; set; }

        [Parameter]
        public int Id { get; set; }

        public LeaveRequestVM LeaveRequest { get; set; }
        public string Message { get; set; } = string.Empty;

        public async Task ChangeApproval(bool approved)
        {
            var response = await LeaveRequestService.ChangeApproval(Id, approved);
            if (response.Success)
            {
                LeaveRequest = await LeaveRequestService.GetLeaveRequestDetails(Id);
            }
            else
            {
                Message = $"Error changing approval: {response.Message} {response.ValidationErrors}";
            }
        }

        protected override async Task OnInitializedAsync()
        {
            LeaveRequest = await LeaveRequestService.GetLeaveRequestDetails(Id);
        }
    }
}
