using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.Leave.Management.Application.Contracts.Persistence;
using Moq;

namespace HR.LeaveManagement.Application.UnitTests.Mocks
{
    public class MoqLeaveAllocationRepository
    {
        public static Mock<ILeaveAllocationRepository> GetLeaveAllocationMoqRepository()
        {
            var leaveAllocations = new List<Leave.Management.Domain.LeaveAllocation>
            {
                new Leave.Management.Domain.LeaveAllocation
                {
                    Id = 1,
                    EmployeeId = "1",
                    LeaveTypeId = 1,
                    NumberOfDays = 5,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now                    
                },
                new Leave.Management.Domain.LeaveAllocation
                {
                    Id = 2,
                    EmployeeId = "2",
                    LeaveTypeId = 2,
                    NumberOfDays = 3,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                },
                new Leave.Management.Domain.LeaveAllocation
                {
                    Id = 3,
                    EmployeeId = "3",
                    LeaveTypeId = 3,
                    NumberOfDays = 1,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                }
            };

            var mockRepo = new Mock<ILeaveAllocationRepository>();

             // Arrange


            mockRepo.Setup(r => r.GetLeaveAllocationsWithDetails()).ReturnsAsync(leaveAllocations);            

            mockRepo.Setup(r => r.GetLeaveAllocationWithDetails(It.IsAny<int>()))
                .ReturnsAsync((int id) => leaveAllocations.First(lt => lt.Id == id));

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Leave.Management.Domain.LeaveAllocation>()))
            .Returns((Leave.Management.Domain.LeaveAllocation leaveAllocation) =>
            {
                var id = leaveAllocations.Max(lt => lt.Id) + 1;
                leaveAllocation.Id = id;
                leaveAllocations.Add(leaveAllocation);                
                return Task.CompletedTask;
            });

            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Leave.Management.Domain.LeaveAllocation>()))
            .Returns((Leave.Management.Domain.LeaveAllocation leaveAllocation) =>
            {
                var existingLeaveAllocation = leaveAllocations.First(lt => lt.Id == leaveAllocation.Id);
                existingLeaveAllocation.EmployeeId = leaveAllocation.EmployeeId;
                existingLeaveAllocation.LeaveTypeId = leaveAllocation.LeaveTypeId;
                existingLeaveAllocation.NumberOfDays = leaveAllocation.NumberOfDays;
                existingLeaveAllocation.ModifiedDate = DateTime.Now;
                return Task.CompletedTask;
            });


            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => leaveAllocations.First(lt => lt.Id == id));

            return mockRepo;
    }
}
}