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
            mockRepo.Setup(repo => repo.GetLeaveRequestWithDetails(It.IsAny<int>())).ReturnsAsync((int id) =>
            {
                return leaveRequests.First(lr => lr.Id == id);
            });

            mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => leaveRequests.FirstOrDefault(lr => lr.Id == id));

            mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Leave.Management.Domain.LeaveRequest>()))
                .Returns((Leave.Management.Domain.LeaveRequest leaveRequest) =>
                {
                    var id = leaveRequests.Max(lr => lr.Id) + 1;
                    leaveRequest.Id = id;
                    leaveRequests.Add(leaveRequest);
                    return Task.CompletedTask;
                });

            mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Leave.Management.Domain.LeaveRequest>()))
                .Returns((Leave.Management.Domain.LeaveRequest leaveRequest) =>
                {
                    var existing = leaveRequests.First(lr => lr.Id == leaveRequest.Id);
                    existing.StartDate = leaveRequest.StartDate;
                    existing.EndDate = leaveRequest.EndDate;
                    existing.LeaveTypeId = leaveRequest.LeaveTypeId;
                    existing.RequestComments = leaveRequest.RequestComments;
                    existing.Approved = leaveRequest.Approved;
                    existing.Cancelled = leaveRequest.Cancelled;
                    existing.ModifiedDate = DateTime.Now;
                    return Task.CompletedTask;
                });

            mockRepo.Setup(repo => repo.DeleteAsync(It.IsAny<Leave.Management.Domain.LeaveRequest>()))
                .Returns((Leave.Management.Domain.LeaveRequest leaveRequest) =>
                {
                    leaveRequests.Remove(leaveRequest);
                    return Task.CompletedTask;
                });

            return mockRepo;
        }
    }
}