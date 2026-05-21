using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models.LeaveTypes;
using Microsoft.AspNetCore.Components;

namespace HR.LeaveManagement.BlazorUI.Pages.LeaveTypes
{
    public partial class Create
    {
        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        private ILeaveTypeService LeaveTypeService { get; set; }

        public LeaveTypeVM LeaveType { get; set; } = new LeaveTypeVM();
        public string Message { get; set; } = string.Empty;

        public async Task CreateLeaveType()
        {
            var response = await LeaveTypeService.CreateLeaveType(LeaveType);
            if (response.Success)
            {
                Navigation.NavigateTo("/leavetypes");
            }
            else
            {
                Message = $"Error creating Leave Type: {response.Message}";
            }
        }

        protected override async Task OnInitializedAsync()
        {
            LeaveType = new LeaveTypeVM();
        }
    }
}
