using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.Leave.Management.Application.Contracts.Persistence;
using Moq;

namespace HR.LeaveManagement.Application.UnitTests.Mocks
{
    public class MoqLeaveTypeRepository
    {
        public static Mock<ILeaveTypeRepository> GetLeaveTypeMoqRepository()
        {
            var leaveTypes = new List<Leave.Management.Domain.LeaveType>
            {
                new Leave.Management.Domain.LeaveType
                {
                    Id = 1,
                    Name = "Test Vacation",
                    DefaultDays = 10,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                },
                new Leave.Management.Domain.LeaveType
                {
                    Id = 2,
                    Name = "Test Sick",
                    DefaultDays = 15,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                },
                new Leave.Management.Domain.LeaveType
                {
                    Id = 3,
                    Name = "Test Day Off",
                    DefaultDays = 1,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                }
            };

            var mockRepo = new Mock<ILeaveTypeRepository>();

             // Arrange
            mockRepo.Setup(r => r.IsLeaveTypeNameUnique(It.IsAny<string>()))
             .ReturnsAsync(true);

            mockRepo.Setup(r => r.LeaveTypeExists(It.IsAny<int>()))
             .ReturnsAsync(true);

            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(leaveTypes);

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Leave.Management.Domain.LeaveType>()))
            .Returns((Leave.Management.Domain.LeaveType leaveType) =>
            {
                var id = leaveTypes.Max(lt => lt.Id) + 1;
                leaveType.Id = id;
                leaveTypes.Add(leaveType);                
                return Task.CompletedTask;
            });

            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Leave.Management.Domain.LeaveType>()))
            .Returns((Leave.Management.Domain.LeaveType leaveType) =>
            {
                var existingLeaveType = leaveTypes.First(lt => lt.Id == leaveType.Id);
                existingLeaveType.Name = leaveType.Name;
                existingLeaveType.DefaultDays = leaveType.DefaultDays;
                existingLeaveType.ModifiedDate = DateTime.Now;
                return Task.CompletedTask;
            });


            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => leaveTypes.First(lt => lt.Id == id));

            return mockRepo;
    }
}
}