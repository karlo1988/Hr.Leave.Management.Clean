using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Features.LeaveAllocation.Queries.GetAllLeaveAllocations;
using HR.Leave.Management.Application.Features.LeaveAllocation.Queries.GetLeaveAllocationDetails;
using HR.Leave.Management.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocations.Queries
{
    public class GetLeaveAllocationDetailsQueryHandlerTests
    {
        private readonly Mock<ILeaveAllocationRepository> _mockRepo;
        private IMapper _mapper;
        private Mock<IAppLogger<GetLeaveAllocationDetailsQueryHandler>> _mockAppLogger;
        public GetLeaveAllocationDetailsQueryHandlerTests()
        {
            _mockRepo = MoqLeaveAllocationRepository.GetLeaveAllocationMoqRepository();
            _mockAppLogger = new Mock<IAppLogger<GetLeaveAllocationDetailsQueryHandler>>();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LeaveAllocationProfile>();
                cfg.AddProfile<LeaveTypeProfile>();
            }, NullLoggerFactory.Instance);

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task GetLeaveAllocationDetailsTest()
        {
            // Arrange
            var handler = new GetLeaveAllocationDetailsQueryHandler(_mockRepo.Object, _mapper, _mockAppLogger.Object);

            // Act
            var result = await handler.Handle(new GetLeaveAllocationDetailsQuery { Id = 1 }, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<LeaveAllocationDetailsDto>();
        }
    }
}