using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.Leave.Management.Application.Contracts.Persistence;
using Moq;

namespace HR.LeaveManagement.Application.UnitTests.Mocks
{
    public class MoqLeaveRequestRepository
    {
       public static Mock<ILeaveRequestRepository> GetLeaveRequestMoqRepository()
        {
            var leaveRequests = new List<Leave.Management.Domain.LeaveRequest>
            {
                new Leave.Management.Domain.LeaveRequest
                {
                    Id = 1,
                    RequestingEmployeeId = "1",
                    StartDate = DateTime.Now.AddDays(10),
                    EndDate = DateTime.Now.AddDays(15),
                    LeaveTypeId = 1,
                    DateRequested = DateTime.Now,
                    RequestComments = "Test Comment 1",
                    Approved = null,
                    Cancelled = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                },
                new Leave.Management.Domain.LeaveRequest
                {
                    Id = 2,
                    RequestingEmployeeId = "2",
                    StartDate = DateTime.Now.AddDays(20),
                    EndDate = DateTime.Now.AddDays(25),
                    LeaveTypeId = 2,
                    DateRequested = DateTime.Now,
                    RequestComments = "Test Comment 2",
                    Approved = null,
                    Cancelled = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                },
                new Leave.Management.Domain.LeaveRequest
                {
                    Id = 3,
                    RequestingEmployeeId = "3",
                    StartDate = DateTime.Now.AddDays(30),
                    EndDate = DateTime.Now.AddDays(35),
                    LeaveTypeId = 3,
                    DateRequested = DateTime.Now,
                    RequestComments = "Test Comment 3",
                    Approved = null,
                    Cancelled = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                }
            };

            var mockRepo = new Mock<ILeaveRequestRepository>();

             // Arrange
            mockRepo.Setup(repo => repo.GetLeaveRequestsWithDetails()).ReturnsAsync(leaveRequests);

            return mockRepo;
        }
    }
}