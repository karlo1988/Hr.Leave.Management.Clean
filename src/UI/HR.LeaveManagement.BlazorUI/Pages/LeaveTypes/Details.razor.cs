using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models.LeaveTypes;
using Microsoft.AspNetCore.Components;

namespace HR.LeaveManagement.BlazorUI.Pages.LeaveTypes
{
    public partial class Details
    {
        [Inject]
        private ILeaveTypeService LeaveTypeService { get; set; }

        [Parameter]
        public int Id { get; set; }

        public LeaveTypeVM LeaveType { get; set; }

        protected override async Task OnInitializedAsync()
        {
            LeaveType = await LeaveTypeService.GetLeaveTypeDetails(Id);
        }
    }
}
