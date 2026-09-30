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
        private readonly Mock<ILeaveAllocationRepository> _mockAllocationRepo;

        public ChangeLeaveRequestApprovalCommandHandlerTests()
        {
            _mockRepo = MoqLeaveRequestRepository.GetLeaveRequestMoqRepository();
            _mockLogger = new Mock<IAppLogger<ChangeLeaveRequestApprovalCommandHandler>>();
            _mockEmailSender = new Mock<IEmailSender>();
            _mockAllocationRepo = new Mock<ILeaveAllocationRepository>();
            SetupAllocation(20);

            _mockEmailSender.Setup(e => e.SendEmail(It.IsAny<Leave.Management.Application.Models.Email.EmailMessage>()))
                .ReturnsAsync(true);
        }

        [Fact]
        public async Task Handle_ApproveRequest_ReturnsUnit()
        {
            // Arrange
            var command = new ChangeLeaveRequestApprovalCommand { Id = 1, Approved = true };
            var handler = new ChangeLeaveRequestApprovalCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockAllocationRepo.Object);

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
            var handler = new ChangeLeaveRequestApprovalCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockAllocationRepo.Object);

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
            var handler = new ChangeLeaveRequestApprovalCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockAllocationRepo.Object);

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
            var handler = new ChangeLeaveRequestApprovalCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockAllocationRepo.Object);

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
            var handler = new ChangeLeaveRequestApprovalCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockAllocationRepo.Object);

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_NullApproved_ThrowsBadRequestException()
        {
            // Arrange
            var command = new ChangeLeaveRequestApprovalCommand { Id = 1, Approved = null };
            var handler = new ChangeLeaveRequestApprovalCommandHandler(_mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockAllocationRepo.Object);

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ApproveRequest_DeductsDaysFromAllocation()
        {
            // Arrange - request 1 spans 6 days
            var allocation = SetupAllocation(20);
            var command = new ChangeLeaveRequestApprovalCommand { Id = 1, Approved = true };
            var handler = CreateHandler();

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            allocation.NumberOfDays.ShouldBe(14);
            _mockAllocationRepo.Verify(r => r.UpdateAsync(allocation), Times.Once);
        }

        [Fact]
        public async Task Handle_RejectPendingRequest_DoesNotChangeAllocation()
        {
            // Arrange
            var allocation = SetupAllocation(20);
            var command = new ChangeLeaveRequestApprovalCommand { Id = 1, Approved = false };
            var handler = CreateHandler();

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            allocation.NumberOfDays.ShouldBe(20);
            _mockAllocationRepo.Verify(r => r.UpdateAsync(It.IsAny<Leave.Management.Domain.LeaveAllocation>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RejectApprovedRequest_RestoresDaysToAllocation()
        {
            // Arrange
            var existing = await _mockRepo.Object.GetByIdAsync(1);
            existing.Approved = true;
            var allocation = SetupAllocation(14);
            var command = new ChangeLeaveRequestApprovalCommand { Id = 1, Approved = false };
            var handler = CreateHandler();

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            allocation.NumberOfDays.ShouldBe(20);
        }

        [Fact]
        public async Task Handle_ApproveWithNotEnoughDays_ThrowsBadRequestException()
        {
            // Arrange
            SetupAllocation(3);
            var command = new ChangeLeaveRequestApprovalCommand { Id = 1, Approved = true };
            var handler = CreateHandler();

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Leave.Management.Domain.LeaveRequest>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ApproveWithoutAllocation_ThrowsBadRequestException()
        {
            // Arrange
            _mockAllocationRepo.Setup(r => r.GetUserAllocations(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((Leave.Management.Domain.LeaveAllocation)null);
            var command = new ChangeLeaveRequestApprovalCommand { Id = 1, Approved = true };
            var handler = CreateHandler();

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_CancelledRequest_ThrowsBadRequestException()
        {
            // Arrange
            var existing = await _mockRepo.Object.GetByIdAsync(1);
            existing.Cancelled = true;
            var command = new ChangeLeaveRequestApprovalCommand { Id = 1, Approved = true };
            var handler = CreateHandler();

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
        }

        private ChangeLeaveRequestApprovalCommandHandler CreateHandler() => new ChangeLeaveRequestApprovalCommandHandler(
            _mockRepo.Object, _mockLogger.Object, _mockEmailSender.Object, _mockAllocationRepo.Object);

        private Leave.Management.Domain.LeaveAllocation SetupAllocation(int numberOfDays)
        {
            var allocation = new Leave.Management.Domain.LeaveAllocation { Id = 1, EmployeeId = "1", NumberOfDays = numberOfDays, LeaveTypeId = 1 };
            _mockAllocationRepo.Setup(r => r.GetUserAllocations(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(allocation);
            return allocation;
        }
    }
}
