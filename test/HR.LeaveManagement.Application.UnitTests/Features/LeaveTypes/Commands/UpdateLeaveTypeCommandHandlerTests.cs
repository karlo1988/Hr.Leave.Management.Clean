using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Features.LeaveType.Commands.CreateLeaveType;
using HR.Leave.Management.Application.Features.LeaveType.Commands.UpdateLeaveType;
using HR.Leave.Management.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveTypes.Commands
{
    public class UpdateLeaveTypeCommandHandlerTests
    {
        private readonly Mock<ILeaveTypeRepository> _mockRepo;
        private IMapper _mapper;
        private Mock<IAppLogger<UpdateLeaveTypeCommandHandler>> _mockAppLogger;
        public UpdateLeaveTypeCommandHandlerTests()
        {
            _mockRepo = MoqLeaveTypeRepository.GetLeaveTypeMoqRepository();
            _mockAppLogger = new Mock<IAppLogger<UpdateLeaveTypeCommandHandler>>();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LeaveTypeProfile>();
            }, NullLoggerFactory.Instance);

            _mapper = configurationProvider.CreateMapper();
        }

         [Fact]
        public async Task UpdateLeaveTypeTest()
        {
            // Arrange
            var handler = new UpdateLeaveTypeCommandHandler(_mockRepo.Object, _mapper, _mockAppLogger.Object);

            // Act            
            var result = await handler.Handle(new UpdateLeaveTypeCommand {  Id = 3, Name = "Day Off", DefaultDays = 1 }, CancellationToken.None);

            // Assert           
            result.ShouldBeOfType<Unit>();

        }
    }
}