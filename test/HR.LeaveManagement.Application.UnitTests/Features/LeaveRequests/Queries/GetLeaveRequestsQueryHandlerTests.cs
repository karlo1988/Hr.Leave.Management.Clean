using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Features.LeaveRequest.Queries.GetAllLeaveRequests;
using HR.Leave.Management.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Queries
{
    public class GetLeaveRequestsQueryHandlerTests
    {
        private readonly Mock<ILeaveRequestRepository> _mockRepo;
        private IMapper _mapper;
        private Mock<IAppLogger<GetLeaveRequestsQueryHandler>> _mockAppLogger;
        public GetLeaveRequestsQueryHandlerTests()
        {
            _mockRepo = MoqLeaveRequestRepository.GetLeaveRequestMoqRepository();
            _mockAppLogger = new Mock<IAppLogger<GetLeaveRequestsQueryHandler>>();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LeaveAllocationProfile>();
                cfg.AddProfile<LeaveTypeProfile>();
                cfg.AddProfile<LeaveRequestProfile>();
            }, NullLoggerFactory.Instance);

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task GetLeaveRequestsListTest()
        {
            // Arrange
            var handler = new GetLeaveRequestsQueryHandler(_mockRepo.Object, _mapper, _mockAppLogger.Object);

            // Act
            var result = await handler.Handle(new GetLeaveRequestsQuery(), CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<LeaveRequestDto>>();
            result.Count.ShouldBe(3);
        }
    }
}