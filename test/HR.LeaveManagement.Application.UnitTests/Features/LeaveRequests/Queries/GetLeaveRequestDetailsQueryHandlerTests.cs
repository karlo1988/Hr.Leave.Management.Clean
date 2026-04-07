using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Features.LeaveRequest.Queries.GetLeaveRequestDetails;
using HR.Leave.Management.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Queries
{
    public class GetLeaveRequestDetailsQueryHandlerTests
    {
        private readonly Mock<ILeaveRequestRepository> _mockRepo;
        private IMapper _mapper;
        public GetLeaveRequestDetailsQueryHandlerTests()
        {
            _mockRepo = MoqLeaveRequestRepository.GetLeaveRequestMoqRepository();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LeaveAllocationProfile>();
                cfg.AddProfile<LeaveTypeProfile>();
                cfg.AddProfile<LeaveRequestProfile>();
            }, NullLoggerFactory.Instance);

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsLeaveRequestDetails()
        {
            // Arrange
            var handler = new GetLeaveRequestDetailsQueryHandler(_mockRepo.Object, _mapper);
            var query = new GetLeaveRequestDetailsQuery { Id = 1 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }
    }
}