using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models.LeaveRequests;
using HR.LeaveManagement.BlazorUI.Models.LeaveTypes;
using Microsoft.AspNetCore.Components;

namespace HR.LeaveManagement.BlazorUI.Pages.LeaveRequests
{
    public partial class Edit
    {
        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        private ILeaveRequestService LeaveRequestService { get; set; }

        [Inject]
        private ILeaveTypeService LeaveTypeService { get; set; }

        [Parameter]
        public int Id { get; set; }

        public LeaveRequestVM LeaveRequest { get; set; }
        public List<LeaveTypeVM> LeaveTypes { get; set; }
        public string Message { get; set; } = string.Empty;

        public async Task UpdateLeaveRequest()
        {
            var response = await LeaveRequestService.UpdateLeaveRequest(Id, LeaveRequest);
            if (response.Success)
            {
                Navigation.NavigateTo("/leaverequests");
            }
            else
            {
                Message = $"Error updating Leave Request: {response.Message} {response.ValidationErrors}";
            }
        }

        protected override async Task OnInitializedAsync()
        {
            LeaveTypes = await LeaveTypeService.GetLeaveTypes();
            LeaveRequest = await LeaveRequestService.GetLeaveRequestDetails(Id);
        }
    }
}
