using System.Threading;
using System.Threading.Tasks;
using HR.Leave.Management.Application.Contracts.Email;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using HR.Leave.Management.Application.Features.LeaveRequest.Commands.CancelLeaveRequest;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands
{
    public class CancelLeaveRequestCommandHandlerTests
    {
        private readonly Mock<ILeaveRequestRepository> _mockRepo;
        private readonly Mock<IAppLogger<CancelLeaveRequestCommandHandler>> _mockLogger;
        private readonly Mock<IEmailSender> _mockEmailSender;

        public CancelLeaveRequestCommandHandlerTests()
        {
            _mockRepo = MoqLeaveRequestRepository.GetLeaveRequestMoqRepository();
            _mockLogger = new Mock<IAppLogger<CancelLeaveRequestCommandHandler>>();
            _mockEmailSender = new Mock<IEmailSender>();

            _mockEmailSender.Setup(e => e.SendEmail(It.IsAny<Leave.Management.Application.Models.Email.EmailMessage>()))
                .ReturnsAsync(true);
        }

        [Fact]
        public async Task Handle_ValidId_ReturnsUnit()
        {
            // Arrange
            var command = new CancelLeaveRequestCommand { Id = 1 };
            var handler = new CancelLeaveRequestCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<Unit>();
        }

        [Fact]
        public async Task Handle_ValidId_SetsCancelledToTrue()
        {
            // Arrange
            var command = new CancelLeaveRequestCommand { Id = 1 };
            var handler = new CancelLeaveRequestCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepo.Verify(r => r.UpdateAsync(It.Is<Leave.Management.Domain.LeaveRequest>(lr => lr.Cancelled == true)), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidId_SendsEmail()
        {
            // Arrange
            var command = new CancelLeaveRequestCommand { Id = 1 };
            var handler = new CancelLeaveRequestCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockEmailSender.Verify(e => e.SendEmail(It.IsAny<Leave.Management.Application.Models.Email.EmailMessage>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentId_ThrowsBadRequestException()
        {
            // Arrange
            var command = new CancelLeaveRequestCommand { Id = 999 };
            var handler = new CancelLeaveRequestCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
