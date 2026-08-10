using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models;
using HR.LeaveManagement.BlazorUI.Services.Base;
using Microsoft.AspNetCore.Components;

namespace HR.LeaveManagement.BlazorUI.Pages
{
    public partial class Login
    {
        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        private IAuthenticationService AuthenticationService { get; set; }

        [SupplyParameterFromQuery(Name = "returnUrl")]
        public string ReturnUrl { get; set; }

        public LoginVM LoginModel { get; set; } = new LoginVM();
        public string Message { get; set; } = string.Empty;
        public bool IsBusy { get; set; }

        public async Task HandleLogin()
        {
            Message = string.Empty;
            IsBusy = true;

            try
            {
                var isLoggedIn = await AuthenticationService.AuthenticateAsync(LoginModel.Email, LoginModel.Password);
                if (isLoggedIn)
                {
                    Navigation.NavigateTo(string.IsNullOrEmpty(ReturnUrl) ? "/" : ReturnUrl);
                    return;
                }

                Message = "Invalid email address or password";
            }
            catch (ApiException ex)
            {
                Message = ex.StatusCode == 404 || ex.StatusCode == 400
                    ? "Invalid email address or password"
                    : ex.GetErrorMessage();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
