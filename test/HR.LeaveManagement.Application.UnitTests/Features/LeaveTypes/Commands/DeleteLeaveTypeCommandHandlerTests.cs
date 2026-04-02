using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Features.LeaveType.Commands.CreateLeaveType;
using HR.Leave.Management.Application.Features.LeaveType.Commands.DeleteLeaveType;
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
    public class DeleteLeaveTypeCommandHandlerTests
    {
        private readonly Mock<ILeaveTypeRepository> _mockRepo;        
        private Mock<IAppLogger<DeleteLeaveTypeCommandHandler>> _mockAppLogger;
        public DeleteLeaveTypeCommandHandlerTests()
        {
            _mockRepo = MoqLeaveTypeRepository.GetLeaveTypeMoqRepository();
            _mockAppLogger = new Mock<IAppLogger<DeleteLeaveTypeCommandHandler>>();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LeaveTypeProfile>();
            }, NullLoggerFactory.Instance);
            
        }

        [Fact]
        public async Task DeleteLeaveTypeTest()
        {
            // Arrange
            var handler = new DeleteLeaveTypeCommandHandler(_mockRepo.Object, _mockAppLogger.Object);

            // Act            
            var result = await handler.Handle(new DeleteLeaveTypeCommand {  Id = 1 }, CancellationToken.None);

            // Assert           
            result.ShouldBeOfType<Unit>();

        }
    }
}