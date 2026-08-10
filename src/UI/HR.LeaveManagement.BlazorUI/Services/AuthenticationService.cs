using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Services.Base;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using HR.LeaveManagement.BlazorUI.Providers;

namespace HR.LeaveManagement.BlazorUI.Services
{
    public class AuthenticationService : BaseHttpService, IAuthenticationService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public AuthenticationService(IClient client,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider) : base(client, localStorage)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            try
            {
                AuthRequest request = new AuthRequest
                {
                    Email = email,
                    Password = password
                };

                AuthResponse response = await _client.LoginAsync(request);

                if (!string.IsNullOrEmpty(response.Token))
                {
                    await _localStorage.SetItemAsync("token", response.Token);
                    //Set claims for Blazor and login state
                    await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedIn();

                    return true;
                }

                return false;
            }
            catch
            {

                //Some login methods
                throw;
            }
        }

        public async Task LogoutAsync()
        {
            //Remove claims for Blazor and invalidate login state
            await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedOut();
        }

        public async Task<bool> RegisterAsync(string firstName, string lastName, string username, string email, string password)
        {
            try
            {
                RegistrationRequest request = new RegistrationRequest
                {
                    FirstName = firstName,
                    LastName = lastName,
                    UserName = username,
                    Email = email,
                    Password = password
                };

                var response = await _client.RegisterAsync(request);

                // The API's register endpoint answers with a RegistrationResponse ({ userId }) even
                // though the generated client types it as AuthResponse, so Id is never populated.
                // Any failure surfaces as an ApiException, so a response at all means we are registered.
                return response != null;
            }
            catch
            {
                throw;
            }
        }
    }
}