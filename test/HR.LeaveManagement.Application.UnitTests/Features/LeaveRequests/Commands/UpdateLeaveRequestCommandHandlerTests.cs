using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Email;
using HR.Leave.Management.Application.Contracts.Identity;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using HR.Leave.Management.Application.Features.LeaveRequest.Commands.UpdateLeaveRequest;
using HR.Leave.Management.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands
{
    public class UpdateLeaveRequestCommandHandlerTests
    {
        private readonly Mock<ILeaveRequestRepository> _mockRepo;
        private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepo;
        private readonly Mock<IAppLogger<UpdateLeaveRequestCommandHandler>> _mockLogger;
        private readonly Mock<IEmailSender> _mockEmailSender;
        private readonly Mock<ILeaveAllocationRepository> _mockAllocationRepo;
        private readonly Mock<IUserService> _mockUserService;
        private readonly IMapper _mapper;

        public UpdateLeaveRequestCommandHandlerTests()
        {
            _mockRepo = MoqLeaveRequestRepository.GetLeaveRequestMoqRepository();
            _mockLeaveTypeRepo = MoqLeaveTypeRepository.GetLeaveTypeMoqRepository();
            _mockLogger = new Mock<IAppLogger<UpdateLeaveRequestCommandHandler>>();
            _mockEmailSender = new Mock<IEmailSender>();
            _mockAllocationRepo = new Mock<ILeaveAllocationRepository>();
            // Request 1 in the mock repository belongs to employee "1"
            _mockUserService = new Mock<IUserService>();
            _mockUserService.Setup(u => u.UserId).Returns("1");
            SetupAllocation(20);

            _mockEmailSender.Setup(e => e.SendEmail(It.IsAny<Leave.Management.Application.Models.Email.EmailMessage>()))
                .ReturnsAsync(true);

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LeaveRequestProfile>();
                cfg.AddProfile<LeaveTypeProfile>();
            }, NullLoggerFactory.Instance);

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsUnit()
        {
            // Arrange
            var command = new UpdateLeaveRequestCommand
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                LeaveTypeId = 1,
                RequestComments = "Updated comment",
                RequestingEmployeeId = "emp-001"
            };

            var handler = new UpdateLeaveRequestCommandHandler(
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockAllocationRepo.Object, _mockUserService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<Unit>();
        }

        [Fact]
        public async Task Handle_ValidCommand_SendsEmail()
        {
            // Arrange
            var command = new UpdateLeaveRequestCommand
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                LeaveTypeId = 1,
                RequestComments = "Updated",
                RequestingEmployeeId = "emp-001"
            };

            var handler = new UpdateLeaveRequestCommandHandler(
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockAllocationRepo.Object, _mockUserService.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockEmailSender.Verify(e => e.SendEmail(It.IsAny<Leave.Management.Application.Models.Email.EmailMessage>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentLeaveRequest_ThrowsBadRequestException()
        {
            // Arrange
            var command = new UpdateLeaveRequestCommand
            {
                Id = 999,
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                LeaveTypeId = 1,
                RequestingEmployeeId = "emp-001"
            };

            var handler = new UpdateLeaveRequestCommandHandler(
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockAllocationRepo.Object, _mockUserService.Object);

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_StartDateAfterEndDate_ThrowsBadRequestException()
        {
            // Arrange
            var command = new UpdateLeaveRequestCommand
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(15),
                EndDate = DateTime.Now.AddDays(5),
                LeaveTypeId = 1,
                RequestingEmployeeId = "emp-001"
            };

            var handler = new UpdateLeaveRequestCommandHandler(
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockAllocationRepo.Object, _mockUserService.Object);

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_OwnPendingDaysAreNotCountedTwice_UpdatesLeaveRequest()
        {
            // Arrange - request 1 is itself a pending 6 day request; changing it to 6 days with 6 allocated must pass
            SetupAllocation(6);

            var command = new UpdateLeaveRequestCommand
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(15),
                LeaveTypeId = 1,
                RequestingEmployeeId = "1"
            };

            var handler = CreateHandler();

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Leave.Management.Domain.LeaveRequest>()), Times.Once);
        }

        [Fact]
        public async Task Handle_MoreDaysThanAllocated_ThrowsBadRequestException()
        {
            // Arrange
            SetupAllocation(3);

            var command = new UpdateLeaveRequestCommand
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(15),
                LeaveTypeId = 1,
                RequestingEmployeeId = "1"
            };

            var handler = CreateHandler();

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Leave.Management.Domain.LeaveRequest>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ApprovedRequest_ThrowsBadRequestException()
        {
            // Arrange
            var existing = await _mockRepo.Object.GetByIdAsync(1);
            existing.Approved = true;

            var command = new UpdateLeaveRequestCommand
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(12),
                LeaveTypeId = 1,
                RequestingEmployeeId = "1"
            };

            var handler = CreateHandler();

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DifferentRequestingEmployee_KeepsOriginalEmployee()
        {
            // Arrange
            var command = new UpdateLeaveRequestCommand
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(12),
                LeaveTypeId = 1,
                RequestingEmployeeId = "someone-else"
            };

            var handler = CreateHandler();

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepo.Verify(r => r.UpdateAsync(It.Is<Leave.Management.Domain.LeaveRequest>(lr => lr.RequestingEmployeeId == "1")), Times.Once);
        }

        private UpdateLeaveRequestCommandHandler CreateHandler() => new UpdateLeaveRequestCommandHandler(
            _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object,
            _mockAllocationRepo.Object, _mockUserService.Object);

        [Fact]
        public async Task Handle_OtherEmployeesRequest_ThrowsNotFoundException()
        {
            // Arrange - request 2 belongs to employee "2"
            var command = new UpdateLeaveRequestCommand
            {
                Id = 2,
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(12),
                LeaveTypeId = 1,
                RequestingEmployeeId = "1"
            };

            var handler = CreateHandler();

            // Act & Assert
            await Should.ThrowAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Leave.Management.Domain.LeaveRequest>()), Times.Never);
        }

        private void SetupAllocation(int numberOfDays)
        {
            _mockAllocationRepo.Setup(r => r.GetUserAllocations(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Leave.Management.Domain.LeaveAllocation { Id = 1, NumberOfDays = numberOfDays, LeaveTypeId = 1 });
        }
    }
}
