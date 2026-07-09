using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Services.Base;
using Blazored.LocalStorage;

namespace HR.LeaveManagement.BlazorUI.Services
{
    public class AuthenticationService : BaseHttpService, IAuthenticationService
    {
        public AuthenticationService(IClient client, ILocalStorageService localStorage) : base(client, localStorage) {}
        
        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            try
            {
                
            
            AuthRequest request = new AuthRequest
            {
                Email = email,
                Password = password
            };

            AuthResponse response = await  _client.LoginAsync(request);

            if(!string.IsNullOrEmpty(response.Token))
            {
                await _localStorage.SetItemAsync("token", response.Token);
                //_client.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", response.Token);

                //Set claims for Blazor and login state

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
            await _localStorage.RemoveItemAsync("token");

            //Remove claims for Blazor and invalidate login state 
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

                if (response.Id != null)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                throw;
            }
        }
    }
}