using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Email;
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
        private readonly IMapper _mapper;

        public UpdateLeaveRequestCommandHandlerTests()
        {
            _mockRepo = MoqLeaveRequestRepository.GetLeaveRequestMoqRepository();
            _mockLeaveTypeRepo = MoqLeaveTypeRepository.GetLeaveTypeMoqRepository();
            _mockLogger = new Mock<IAppLogger<UpdateLeaveRequestCommandHandler>>();
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
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

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
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

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
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

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
                _mapper, _mockRepo.Object, _mockLeaveTypeRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
