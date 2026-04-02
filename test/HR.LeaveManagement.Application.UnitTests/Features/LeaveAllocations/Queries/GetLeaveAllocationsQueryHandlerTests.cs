using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Features.LeaveAllocation.Queries.GetAllLeaveAllocations;
using HR.Leave.Management.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocations.Queries
{
    public class GetLeaveAllocationsQueryHandlerTests
    {
        private readonly Mock<ILeaveAllocationRepository> _mockRepo;
        private IMapper _mapper;
        private Mock<IAppLogger<GetLeaveAllocationsQueryHandler>> _mockAppLogger;
        public GetLeaveAllocationsQueryHandlerTests()
        {
            _mockRepo = MoqLeaveAllocationRepository.GetLeaveAllocationMoqRepository();
            _mockAppLogger = new Mock<IAppLogger<GetLeaveAllocationsQueryHandler>>();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LeaveAllocationProfile>();
                cfg.AddProfile<LeaveTypeProfile>();
            }, NullLoggerFactory.Instance);

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task GetLeaveAllocationsListTest()
        {
            // Arrange
            var handler = new GetLeaveAllocationsQueryHandler(_mockRepo.Object, _mapper, _mockAppLogger.Object);

            // Act
            var result = await handler.Handle(new GetLeaveAllocationsQuery(), CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<LeaveAllocationDto>>();
            result.Count.ShouldBe(3);
        }
    }
}