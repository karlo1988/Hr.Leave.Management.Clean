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

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveTypes.Queries
{
    public class GetLeaveTypeListQueryHandlerTests
    {
        private readonly Mock<ILeaveTypeRepository> _mockRepo;
        private IMapper _mapper;
        private Mock<IAppLogger<GetLeaveTypesQueryHandler>> _mockAppLogger;
        public GetLeaveTypeListQueryHandlerTests()
        {
            _mockRepo = MoqLeaveTypeRepository.GetLeaveTypeMoqRepository();
            _mockAppLogger = new Mock<IAppLogger<GetLeaveTypesQueryHandler>>();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LeaveTypeProfile>();
            }, NullLoggerFactory.Instance);

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task GetLeaveTypeListTest()
        {
            // Arrange
            var handler = new GetLeaveTypesQueryHandler(_mockRepo.Object, _mapper, _mockAppLogger.Object);

            // Act
            var result = await handler.Handle(new GetLeaveTypesQuery(), CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<LeaveTypeDto>>();
        }
    }
}
