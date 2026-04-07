using System.Threading;
using System.Threading.Tasks;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Exceptions;
using HR.Leave.Management.Application.Features.LeaveRequest.Commands.DeleteLeaveRequest;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands
{
    public class DeleteLeaveRequestCommandHandlerTests
    {
        private readonly Mock<ILeaveRequestRepository> _mockRepo;
        private readonly Mock<IAppLogger<DeleteLeaveRequestCommandHandler>> _mockLogger;

        public DeleteLeaveRequestCommandHandlerTests()
        {
            _mockRepo = MoqLeaveRequestRepository.GetLeaveRequestMoqRepository();
            _mockLogger = new Mock<IAppLogger<DeleteLeaveRequestCommandHandler>>();
        }

        [Fact]
        public async Task Handle_ValidId_ReturnsUnit()
        {
            // Arrange
            var command = new DeleteLeaveRequestCommand { Id = 1 };
            var handler = new DeleteLeaveRequestCommandHandler(_mockRepo.Object, _mockLogger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<Unit>();
        }

        [Fact]
        public async Task Handle_ValidId_CallsDeleteAsync()
        {
            // Arrange
            var command = new DeleteLeaveRequestCommand { Id = 1 };
            var handler = new DeleteLeaveRequestCommandHandler(_mockRepo.Object, _mockLogger.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Leave.Management.Domain.LeaveRequest>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentId_ThrowsNotFoundException()
        {
            // Arrange
            var command = new DeleteLeaveRequestCommand { Id = 999 };
            var handler = new DeleteLeaveRequestCommandHandler(_mockRepo.Object, _mockLogger.Object);

            // Act & Assert
            await Should.ThrowAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
