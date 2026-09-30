using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.Leave.Management.Application.Contracts.Identity;
using HR.Leave.Management.Application.Models.Identity;
using HR.LeaveManagement.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace HR.LeaveManagement.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // "uid" is the claim AuthService puts the user's Id in when generating the JWT
        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirst("uid")?.Value ?? string.Empty;

        public async Task<Employee> GetEmployee(string userId)
        {
            var emaployee = await _userManager.FindByIdAsync(userId);
            if (emaployee == null)
            {
                throw new Exception($"Employee with id {userId} not found.");
            }

            return new Employee
            {
                Id = emaployee.Id,
                FirstName = emaployee.FirstName,
                LastName = emaployee.LastName,
                Email = emaployee.Email, 
               
            };
        }

        public async Task<List<Employee>> GetEmployees()
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");

            return employees.Select(e => new Employee
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email
            }).ToList();
        }
    }
}