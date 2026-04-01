using HR.Leave.Management.Application.MappingProfiles;
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Features.LeaveType.Queries.GettAllLeaveTypes;
using Xunit;
using Shouldly;
using HR.Leave.Management.Application.Features.LeaveType.Queries.GetLeaveTypeDetails;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveTypes.Queries
{
    public class GetLeaveTypeDetailsQueryHandlerTests
    {
        private readonly Mock<ILeaveTypeRepository> _mockRepo;
        private IMapper _mapper;
        private Mock<IAppLogger<GetLeaveTypeDetailsQueryHandler>> _mockAppLogger;
        public GetLeaveTypeDetailsQueryHandlerTests()
        {
            _mockRepo = MoqLeaveTypeRepository.GetLeaveTypeMoqRepository();
            _mockAppLogger = new Mock<IAppLogger<GetLeaveTypeDetailsQueryHandler>>();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LeaveTypeProfile>();
            }, NullLoggerFactory.Instance);

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task GetLeaveTypeDetailsTest()
        {
            // Arrange
            var handler = new GetLeaveTypeDetailsQueryHandler(_mockRepo.Object, _mapper, _mockAppLogger.Object);

            // Act
            var result = await handler.Handle(new GetLeaveTypeDetailsQuery { Id = 1 }, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<GetLeaveTypeDetailsDto>();
        }
    }
}
