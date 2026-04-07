using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Email;
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
        private readonly IMapper _mapper;

        public CreateLeaveRequestCommandHandlerTests()
        {
            _mockRepo = MoqLeaveRequestRepository.GetLeaveRequestMoqRepository();
            _mockLeaveTypeRepo = MoqLeaveTypeRepository.GetLeaveTypeMoqRepository();
            _mockLogger = new Mock<IAppLogger<CreateLeaveRequestCommandHandler>>();
            _mockEmailSender = new Mock<IEmailSender>();

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
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

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
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

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
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

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
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_MissingEmployeeId_ThrowsBadRequestException()
        {
            // Arrange
            var command = new CreateLeaveRequestCommand
            {
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(10),
                LeaveTypeId = 1,
                RequestingEmployeeId = string.Empty
            };

            var handler = new CreateLeaveRequestCommandHandler(
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
