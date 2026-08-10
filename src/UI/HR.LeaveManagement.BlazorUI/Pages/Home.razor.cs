using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Providers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace HR.LeaveManagement.BlazorUI.Pages
{
    public partial class Home
    {
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }

        protected override async Task OnInitializedAsync()
        {
             await ((ApiAuthenticationStateProvider) AuthenticationStateProvider).GetAuthenticationStateAsync();
        }

        protected void GoToLogin()
        {
            NavigationManager.NavigateTo("login/");
        }

         protected void GoToRegister()
        {
            NavigationManager.NavigateTo("register/");
        }

        protected async Task Logout()
        {
            await AuthenticationService.LogoutAsync();
            //await ((ApiAuthenticationStateProvider) AuthenticationStateProvider).LoggedOut();
            //NavigationManager.NavigateTo("login/");
        }
    
    }
}