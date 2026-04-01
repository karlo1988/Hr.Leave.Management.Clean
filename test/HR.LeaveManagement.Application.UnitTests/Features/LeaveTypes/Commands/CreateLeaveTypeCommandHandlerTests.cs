using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Features.LeaveType.Commands.CreateLeaveType;
using HR.Leave.Management.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveTypes.Commands
{
    public class CreateLeaveTypeCommandHandlerTests
    {
        private readonly Mock<ILeaveTypeRepository> _mockRepo;
        private IMapper _mapper;
        private Mock<IAppLogger<CreateLeaveTypeCommandHandler>> _mockAppLogger;
        public CreateLeaveTypeCommandHandlerTests()
        {
            _mockRepo = MoqLeaveTypeRepository.GetLeaveTypeMoqRepository();
            _mockAppLogger = new Mock<IAppLogger<CreateLeaveTypeCommandHandler>>();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LeaveTypeProfile>();
            }, NullLoggerFactory.Instance);

            _mapper = configurationProvider.CreateMapper();
        }

         [Fact]
        public async Task CreateLeaveTypeTest()
        {
            // Arrange
            var handler = new CreateLeaveTypeCommandHandler(_mockRepo.Object, _mapper, _mockAppLogger.Object);

            // Act            
            var result = await handler.Handle(new CreateLeaveTypeCommand {  Name = "Test Leave Type", DefaultDays = 10 }, CancellationToken.None);

            // Assert           
            result.ShouldBeOfType<int>();
            result.ShouldBeGreaterThan(0);
        }
    }
}