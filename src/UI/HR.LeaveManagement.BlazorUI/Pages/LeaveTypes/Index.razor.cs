using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models.LeaveTypes;
using Microsoft.AspNetCore.Components;

namespace HR.LeaveManagement.BlazorUI.Pages.LeaveTypes
{
    public partial class Index
    {

        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        private ILeaveTypeService LeaveTypeService { get; set; }

        public List<LeaveTypeVM> LeaveTypes { get; set; }
        public string Message { get; set; } = string.Empty;
        public void NavigateToCreate()
        {
            Navigation.NavigateTo("/leavetypes/create");
        }

        public void NavigateToEdit(int id)
        {
            Navigation.NavigateTo($"/leavetypes/edit/{id}");
        }

        public async Task DeleteLeaveType(int id)
        {
            var response = await LeaveTypeService.DeleteLeaveType(id);
            if (response.Success)
            {                
                StateHasChanged();                
            }
            else
            {
                Message = $"Error deleting Leave Type: {response.Message}";
            }
        }

        public void NavigateToDetails(int id)
        {
            Navigation.NavigateTo($"/leavetypes/details/{id}");
        }

        public void AllocateLeaveType(int id)
        {
            //Navigation.NavigateTo($"/leavetypes/allocate/{id}");
        }

        protected override async Task OnInitializedAsync()
        {
            LeaveTypes = await LeaveTypeService.GetLeaveTypes();
        }
      
    }
}