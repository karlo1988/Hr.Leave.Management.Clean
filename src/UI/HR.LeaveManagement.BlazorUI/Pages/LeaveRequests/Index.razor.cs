using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models.LeaveRequests;
using Microsoft.AspNetCore.Components;

namespace HR.LeaveManagement.BlazorUI.Pages.LeaveRequests
{
    public partial class Index
    {
        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        private ILeaveRequestService LeaveRequestService { get; set; }

        public List<LeaveRequestVM> LeaveRequests { get; set; }
        public string Message { get; set; } = string.Empty;

        public void NavigateToCreate()
        {
            Navigation.NavigateTo("/leaverequests/create");
        }

        public void NavigateToEdit(int id)
        {
            Navigation.NavigateTo($"/leaverequests/edit/{id}");
        }

        public void NavigateToDetails(int id)
        {
            Navigation.NavigateTo($"/leaverequests/details/{id}");
        }

        public async Task DeleteLeaveRequest(int id)
        {
            var response = await LeaveRequestService.DeleteLeaveRequest(id);
            if (response.Success)
            {
                LeaveRequests = await LeaveRequestService.GetLeaveRequests();
                StateHasChanged();
            }
            else
            {
                Message = $"Error deleting Leave Request: {response.Message}";
            }
        }

        public async Task ChangeApproval(int id, bool approved)
        {
            var response = await LeaveRequestService.ChangeApproval(id, approved);
            if (response.Success)
            {
                LeaveRequests = await LeaveRequestService.GetLeaveRequests();
                StateHasChanged();
            }
            else
            {
                Message = $"Error changing approval: {response.Message} {response.ValidationErrors}";
            }
        }

        public static string GetStatusCssClass(LeaveRequestVM leaveRequest) => leaveRequest.Status switch
        {
            "Approved" => "bg-success",
            "Rejected" => "bg-danger",
            "Cancelled" => "bg-secondary",
            _ => "bg-warning text-dark"
        };

        protected override async Task OnInitializedAsync()
        {
            LeaveRequests = await LeaveRequestService.GetLeaveRequests();
        }
    }
}
