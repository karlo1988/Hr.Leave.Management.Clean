using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models.LeaveRequests;
using HR.LeaveManagement.BlazorUI.Models.LeaveTypes;
using Microsoft.AspNetCore.Components;

namespace HR.LeaveManagement.BlazorUI.Pages.LeaveRequests
{
    public partial class Create
    {
        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        private ILeaveRequestService LeaveRequestService { get; set; }

        [Inject]
        private ILeaveTypeService LeaveTypeService { get; set; }

        public LeaveRequestVM LeaveRequest { get; set; } = new LeaveRequestVM();
        public List<LeaveTypeVM> LeaveTypes { get; set; }
        public string Message { get; set; } = string.Empty;

        public async Task CreateLeaveRequest()
        {
            var response = await LeaveRequestService.CreateLeaveRequest(LeaveRequest);
            if (response.Success)
            {
                Navigation.NavigateTo("/leaverequests");
            }
            else
            {
                Message = $"Error creating Leave Request: {response.Message} {response.ValidationErrors}";
            }
        }

        protected override async Task OnInitializedAsync()
        {
            LeaveRequest = new LeaveRequestVM
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            };
            LeaveTypes = await LeaveTypeService.GetLeaveTypes();
        }
    }
}
