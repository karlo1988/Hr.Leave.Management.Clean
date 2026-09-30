using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.Leave.Management.Application.Contracts.Identity;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using HR.Leave.Management.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation;
using HR.Leave.Management.Application.Models.Identity;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocations.Commands
{
    public class CreateLeaveAllocationCommandHandlerTests
    {
        private readonly Mock<ILeaveAllocationRepository> _mockRepo;
        private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepo;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IAppLogger<CreateLeaveAllocationCommandHandler>> _mockAppLogger;
        private List<Leave.Management.Domain.LeaveAllocation> _addedAllocations;

        public CreateLeaveAllocationCommandHandlerTests()
        {
            _mockRepo = MoqLeaveAllocationRepository.GetLeaveAllocationMoqRepository();
            _mockLeaveTypeRepo = MoqLeaveTypeRepository.GetLeaveTypeMoqRepository();
            _mockAppLogger = new Mock<IAppLogger<CreateLeaveAllocationCommandHandler>>();

            _mockUserService = new Mock<IUserService>();
            _mockUserService.Setup(u => u.GetEmployees()).ReturnsAsync(new List<Employee>
            {
                new Employee { Id = "emp-1" },
                new Employee { Id = "emp-2" }
            });

            _mockRepo.Setup(r => r.AllocationExists(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(false);
            _mockRepo.Setup(r => r.AddAllocations(It.IsAny<List<Leave.Management.Domain.LeaveAllocation>>()))
                .Callback((List<Leave.Management.Domain.LeaveAllocation> allocations) => _addedAllocations = allocations)
                .Returns(Task.CompletedTask);
        }

        private CreateLeaveAllocationCommandHandler CreateHandler() => new CreateLeaveAllocationCommandHandler(
            _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockUserService.Object, _mockAppLogger.Object);

        [Fact]
        public async Task Handle_ValidLeaveType_AllocatesDefaultDaysToEveryEmployee()
        {
            // Arrange
            var leaveType = await _mockLeaveTypeRepo.Object.GetByIdAsync(1);
            var command = new CreateLeaveAllocationCommand { LeaveTypeId = 1 };

            // Act
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBe(2);
            _addedAllocations.Select(a => a.EmployeeId).ShouldBe(new[] { "emp-1", "emp-2" });
            _addedAllocations.ShouldAllBe(a => a.LeaveTypeId == 1
                                               && a.NumberOfDays == leaveType.DefaultDays
                                               && a.Period == DateTime.UtcNow.Year);
        }

        [Fact]
        public async Task Handle_EmployeeAlreadyAllocated_SkipsThatEmployee()
        {
            // Arrange
            _mockRepo.Setup(r => r.AllocationExists("emp-1", 1, It.IsAny<int>())).ReturnsAsync(true);
            var command = new CreateLeaveAllocationCommand { LeaveTypeId = 1 };

            // Act
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBe(1);
            _addedAllocations.Single().EmployeeId.ShouldBe("emp-2");
        }

        [Fact]
        public async Task Handle_EveryoneAlreadyAllocated_AddsNothing()
        {
            // Arrange
            _mockRepo.Setup(r => r.AllocationExists(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            var command = new CreateLeaveAllocationCommand { LeaveTypeId = 1 };

            // Act
            var result = await CreateHandler().Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBe(0);
            _mockRepo.Verify(r => r.AddAllocations(It.IsAny<List<Leave.Management.Domain.LeaveAllocation>>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NonExistentLeaveType_ThrowsBadRequestException()
        {
            // Arrange
            _mockLeaveTypeRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Leave.Management.Domain.LeaveType)null);
            var command = new CreateLeaveAllocationCommand { LeaveTypeId = 999 };

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => CreateHandler().Handle(command, CancellationToken.None));
        }
    }
}
