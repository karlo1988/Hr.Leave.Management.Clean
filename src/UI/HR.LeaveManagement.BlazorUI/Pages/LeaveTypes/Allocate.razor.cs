using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models.LeaveTypes;
using Microsoft.AspNetCore.Components;

namespace HR.LeaveManagement.BlazorUI.Pages.LeaveTypes
{
    public partial class Allocate
    {
        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        private ILeaveTypeService LeaveTypeService { get; set; }

        [Parameter]
        public int Id { get; set; }

        public LeaveTypeVM LeaveType { get; set; }
        public string Message { get; set; } = string.Empty;

        public void AllocateLeaveType()
        {
            // ILeaveAllocationService.AllocateLeaveType not yet implemented
            Navigation.NavigateTo("/leavetypes");
        }

        protected override async Task OnInitializedAsync()
        {
            LeaveType = await LeaveTypeService.GetLeaveTypeDetails(Id);
        }
    }
}
