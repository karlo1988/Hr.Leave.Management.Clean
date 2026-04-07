using System.Threading;
using System.Threading.Tasks;
using HR.Leave.Management.Application.Contracts.Email;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using HR.Leave.Management.Application.Features.LeaveRequest.Commands.ChangeLeaveRequestApproval;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands
{
    public class ChangeLeaveRequestApprovalCommandHandlerTests
    {
        private readonly Mock<ILeaveRequestRepository> _mockRepo;
        private readonly Mock<IAppLogger<ChangeLeaveRequestApprovalCommandHandler>> _mockLogger;
        private readonly Mock<IEmailSender> _mockEmailSender;

        public ChangeLeaveRequestApprovalCommandHandlerTests()
        {
            _mockRepo = MoqLeaveRequestRepository.GetLeaveRequestMoqRepository();
            _mockLogger = new Mock<IAppLogger<ChangeLeaveRequestApprovalCommandHandler>>();
            _mockEmailSender = new Mock<IEmailSender>();

            _mockEmailSender.Setup(e => e.SendEmail(It.IsAny<Leave.Management.Application.Models.Email.EmailMessage>()))
                .ReturnsAsync(true);
        }

        [Fact]
        public async Task Handle_ApproveRequest_ReturnsUnit()
        {
            // Arrange
            var command = new ChangeLeaveRequestApprovalCommand { Id = 1, Approved = true };
            var handler = new ChangeLeaveRequestApprovalCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<Unit>();
        }

        [Fact]
        public async Task Handle_ApproveRequest_SetsApprovedToTrue()
        {
            // Arrange
            var command = new ChangeLeaveRequestApprovalCommand { Id = 1, Approved = true };
            var handler = new ChangeLeaveRequestApprovalCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepo.Verify(r => r.UpdateAsync(It.Is<Leave.Management.Domain.LeaveRequest>(lr => lr.Approved == true)), Times.Once);
        }

        [Fact]
        public async Task Handle_RejectRequest_SetsApprovedToFalse()
        {
            // Arrange
            var command = new ChangeLeaveRequestApprovalCommand { Id = 1, Approved = false };
            var handler = new ChangeLeaveRequestApprovalCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepo.Verify(r => r.UpdateAsync(It.Is<Leave.Management.Domain.LeaveRequest>(lr => lr.Approved == false)), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidCommand_SendsEmail()
        {
            // Arrange
            var command = new ChangeLeaveRequestApprovalCommand { Id = 1, Approved = true };
            var handler = new ChangeLeaveRequestApprovalCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockEmailSender.Verify(e => e.SendEmail(It.IsAny<Leave.Management.Application.Models.Email.EmailMessage>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentId_ThrowsBadRequestException()
        {
            // Arrange
            var command = new ChangeLeaveRequestApprovalCommand { Id = 999, Approved = true };
            var handler = new ChangeLeaveRequestApprovalCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_NullApproved_ThrowsBadRequestException()
        {
            // Arrange
            var command = new ChangeLeaveRequestApprovalCommand { Id = 1, Approved = null };
            var handler = new ChangeLeaveRequestApprovalCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object);

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
