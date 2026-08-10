using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models;
using HR.LeaveManagement.BlazorUI.Services.Base;
using Microsoft.AspNetCore.Components;

namespace HR.LeaveManagement.BlazorUI.Pages
{
    public partial class Register
    {
        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        private IAuthenticationService AuthenticationService { get; set; }

        public RegisterVM RegisterModel { get; set; } = new RegisterVM();
        public string Message { get; set; } = string.Empty;
        public bool IsBusy { get; set; }

        public async Task HandleRegistration()
        {
            Message = string.Empty;
            IsBusy = true;

            try
            {
                var isRegistered = await AuthenticationService.RegisterAsync(
                    RegisterModel.FirstName,
                    RegisterModel.LastName,
                    RegisterModel.UserName,
                    RegisterModel.Email,
                    RegisterModel.Password);

                if (isRegistered)
                {
                    Navigation.NavigateTo("/login");
                    return;
                }

                Message = "Registration failed, please try again";
            }
            catch (ApiException ex)
            {
                Message = ex.GetErrorMessage();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
