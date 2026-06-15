using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models.LeaveTypes;
using Microsoft.AspNetCore.Components;

namespace HR.LeaveManagement.BlazorUI.Pages.LeaveTypes
{
    public partial class Edit
    {
        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        private ILeaveTypeService LeaveTypeService { get; set; }

        [Parameter]
        public int Id { get; set; }

        public LeaveTypeVM LeaveType { get; set; }
        public string Message { get; set; } = string.Empty;

        public async Task UpdateLeaveType()
        {
            var response = await LeaveTypeService.UpdateLeaveType(Id, LeaveType);
            if (response.Success)
            {
                Navigation.NavigateTo("/leavetypes");
            }
            else
            {
                Message = $"Error updating Leave Type: {response.Message} { response.ValidationErrors}";
            }
        }

        protected override async Task OnInitializedAsync()
        {
            LeaveType = await LeaveTypeService.GetLeaveTypeDetails(Id);
        }
    }
}
