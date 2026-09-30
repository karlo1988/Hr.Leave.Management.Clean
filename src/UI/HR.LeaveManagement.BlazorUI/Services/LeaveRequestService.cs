using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models.LeaveRequests;
using HR.LeaveManagement.BlazorUI.Services.Base;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
namespace HR.LeaveManagement.BlazorUI.Services
{
    public class LeaveRequestService: BaseHttpService, ILeaveRequestService
    {
        private readonly IMapper _mapper;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public LeaveRequestService(IClient client, IMapper mapper, ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider) : base(client, localStorage)
        {
            _mapper = mapper;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<Response<Guid>> CreateLeaveRequest(LeaveRequestVM leaveRequest)
        {
            try
            {
                // The API sets RequestingEmployeeId from the logged in user's token
                var createLeaveRequestCommand = _mapper.Map<CreateLeaveRequestCommand>(leaveRequest);
                await AddBearerToken();
                await _client.LeaveRequestsPOSTAsync(createLeaveRequestCommand);
                return new Response<Guid>() { Success = true };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<Guid>(ex);
            }
        }

        public async Task<Response<Guid>> DeleteLeaveRequest(int id)
        {
            try
            {
                await AddBearerToken();
                await _client.LeaveRequestsDELETEAsync(id);
                return new Response<Guid>() { Success = true };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<Guid>(ex);
            }
        }

        public async Task<LeaveRequestVM> GetLeaveRequestDetails(int id)
        {
            await AddBearerToken();
            var leaveRequest = await _client.LeaveRequestsGETAsync(id);
            return _mapper.Map<LeaveRequestVM>(leaveRequest);
        }

        public async Task<List<LeaveRequestVM>> GetLeaveRequests()
        {
            await AddBearerToken();
            var leaveRequests = await _client.LeaveRequestsAllAsync();
            return _mapper.Map<List<LeaveRequestVM>>(leaveRequests);
        }

        public async Task<Response<Guid>> UpdateLeaveRequest(int id, LeaveRequestVM leaveRequest)
        {
            try
            {
                var updateLeaveRequestCommand = _mapper.Map<UpdateLeaveRequestCommand>(leaveRequest);
                // The API reads the id from the body, so keep it in sync with the route
                updateLeaveRequestCommand.Id = id;
                if (string.IsNullOrEmpty(updateLeaveRequestCommand.RequestingEmployeeId))
                    updateLeaveRequestCommand.RequestingEmployeeId = await GetCurrentUserId();
                await AddBearerToken();
                await _client.LeaveRequestsPUTAsync(id, updateLeaveRequestCommand);
                return new Response<Guid>() { Success = true };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<Guid>(ex);
            }
        }

        public async Task<Response<Guid>> ChangeApproval(int id, bool approved)
        {
            try
            {
                var command = new ChangeLeaveRequestApprovalCommand { Id = id, Approved = approved };
                await AddBearerToken();
                await _client.ApprovalAsync(id, command);
                return new Response<Guid>() { Success = true };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<Guid>(ex);
            }
        }

        private async Task<string> GetCurrentUserId()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return authState.User.FindFirst("uid")?.Value ?? string.Empty;
        }
    }
}
