using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Email;
using HR.Leave.Management.Application.Contracts.Identity;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using HR.Leave.Management.Application.Features.LeaveRequest.Commands.CreateLeaveRequest;
using HR.Leave.Management.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands
{
    public class CreateLeaveRequestCommandHandlerTests
    {
        private readonly Mock<ILeaveRequestRepository> _mockRepo;
        private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepo;
        private readonly Mock<IAppLogger<CreateLeaveRequestCommandHandler>> _mockLogger;
        private readonly Mock<IEmailSender> _mockEmailSender;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILeaveAllocationRepository> _mockAllocationRepo;
        private readonly IMapper _mapper;

        public CreateLeaveRequestCommandHandlerTests()
        {
            _mockRepo = MoqLeaveRequestRepository.GetLeaveRequestMoqRepository();
            _mockLeaveTypeRepo = MoqLeaveTypeRepository.GetLeaveTypeMoqRepository();
            _mockLogger = new Mock<IAppLogger<CreateLeaveRequestCommandHandler>>();
            _mockEmailSender = new Mock<IEmailSender>();
            _mockUserService = new Mock<IUserService>();
            _mockUserService.Setup(u => u.UserId).Returns("emp-001");
            _mockAllocationRepo = new Mock<ILeaveAllocationRepository>();
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
        public async Task Handle_ValidCommand_ReturnsNewLeaveRequestId()
        {
            // Arrange
            var command = new CreateLeaveRequestCommand
            {
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                LeaveTypeId = 1,
                RequestComments = "Need a vacation",
                RequestingEmployeeId = "emp-001"
            };

            var handler = new CreateLeaveRequestCommandHandler(
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockUserService.Object, _mockAllocationRepo.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<int>();
            result.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Handle_ValidCommand_SendsEmail()
        {
            // Arrange
            var command = new CreateLeaveRequestCommand
            {
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                LeaveTypeId = 1,
                RequestComments = "Vacation",
                RequestingEmployeeId = "emp-001"
            };

            var handler = new CreateLeaveRequestCommandHandler(
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockUserService.Object, _mockAllocationRepo.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockEmailSender.Verify(e => e.SendEmail(It.IsAny<Leave.Management.Application.Models.Email.EmailMessage>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidLeaveTypeId_ThrowsBadRequestException()
        {
            // Arrange
            _mockLeaveTypeRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Leave.Management.Domain.LeaveType)null);

            var command = new CreateLeaveRequestCommand
            {
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                LeaveTypeId = 999,
                RequestingEmployeeId = "emp-001"
            };

            var handler = new CreateLeaveRequestCommandHandler(
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockUserService.Object, _mockAllocationRepo.Object);

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_StartDateAfterEndDate_ThrowsBadRequestException()
        {
            // Arrange
            var command = new CreateLeaveRequestCommand
            {
                StartDate = DateTime.Now.AddDays(15),
                EndDate = DateTime.Now.AddDays(5),
                LeaveTypeId = 1,
                RequestingEmployeeId = "emp-001"
            };

            var handler = new CreateLeaveRequestCommandHandler(
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockUserService.Object, _mockAllocationRepo.Object);

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_MissingEmployeeId_ThrowsBadRequestException()
        {
            // Arrange
            _mockUserService.Setup(u => u.UserId).Returns(string.Empty);

            var command = new CreateLeaveRequestCommand
            {
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                LeaveTypeId = 1,
                RequestingEmployeeId = string.Empty
            };

            var handler = new CreateLeaveRequestCommandHandler(
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockUserService.Object, _mockAllocationRepo.Object);

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_NoAllocationForLeaveType_ThrowsBadRequestException()
        {
            // Arrange
            _mockAllocationRepo.Setup(r => r.GetUserAllocations(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((Leave.Management.Domain.LeaveAllocation)null);

            var command = new CreateLeaveRequestCommand
            {
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                LeaveTypeId = 1
            };

            var handler = CreateHandler();

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Leave.Management.Domain.LeaveRequest>()), Times.Never);
        }

        [Fact]
        public async Task Handle_MoreDaysThanAllocated_ThrowsBadRequestException()
        {
            // Arrange - 6 days requested, 5 allocated
            SetupAllocation(5);

            var command = new CreateLeaveRequestCommand
            {
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                LeaveTypeId = 1
            };

            var handler = CreateHandler();

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Leave.Management.Domain.LeaveRequest>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ExactlyAllocatedDays_CreatesLeaveRequest()
        {
            // Arrange - 6 days requested (both first and last day count), 6 allocated
            SetupAllocation(6);

            var command = new CreateLeaveRequestCommand
            {
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                LeaveTypeId = 1
            };

            var handler = CreateHandler();

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Leave.Management.Domain.LeaveRequest>()), Times.Once);
        }

        [Fact]
        public async Task Handle_PendingRequestsUseUpBalance_ThrowsBadRequestException()
        {
            // Arrange - employee "1" already has a pending 6 day request for leave type 1 in the mock repository,
            // so 10 allocated days leave 4 available and a 5 day request must fail
            _mockUserService.Setup(u => u.UserId).Returns("1");
            SetupAllocation(10);

            var command = new CreateLeaveRequestCommand
            {
                StartDate = DateTime.Now.AddDays(11),
                EndDate = DateTime.Now.AddDays(15),
                LeaveTypeId = 1
            };

            var handler = CreateHandler();

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ValidCommand_UsesLoggedInUserAsRequestingEmployee()
        {
            // Arrange
            var command = new CreateLeaveRequestCommand
            {
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                LeaveTypeId = 1,
                RequestingEmployeeId = "someone-else"
            };

            var handler = CreateHandler();

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepo.Verify(r => r.AddAsync(It.Is<Leave.Management.Domain.LeaveRequest>(lr => lr.RequestingEmployeeId == "emp-001")), Times.Once);
        }

        private CreateLeaveRequestCommandHandler CreateHandler() => new CreateLeaveRequestCommandHandler(
            _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object,
            _mockUserService.Object, _mockAllocationRepo.Object);

        private void SetupAllocation(int numberOfDays)
        {
            _mockAllocationRepo.Setup(r => r.GetUserAllocations(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Leave.Management.Domain.LeaveAllocation { Id = 1, NumberOfDays = numberOfDays, LeaveTypeId = 1 });
        }
    }
}
