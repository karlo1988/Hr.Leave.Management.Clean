using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation;
using HR.Leave.Management.Application.Features.LeaveAllocation.Commands.DeleteLeaveAllocation;
using HR.Leave.Management.Application.Features.LeaveAllocation.Commands.UpdateLeaveAllocation;
using HR.Leave.Management.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocations.Commands
{
    public class DeleteLeaveAllocationCommandHandlerTests
    {
        private readonly Mock<ILeaveAllocationRepository> _mockRepo;
        private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepo;
        private IMapper _mapper;
        private Mock<IAppLogger<DeleteLeaveAllocationCommandHandler>> _mockAppLogger;
        public DeleteLeaveAllocationCommandHandlerTests()
        {
            _mockRepo = MoqLeaveAllocationRepository.GetLeaveAllocationMoqRepository();
            _mockLeaveTypeRepo = MoqLeaveTypeRepository.GetLeaveTypeMoqRepository();
            _mockAppLogger = new Mock<IAppLogger<DeleteLeaveAllocationCommandHandler>>();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LeaveAllocationProfile>();
                cfg.AddProfile<LeaveTypeProfile>();
            }, NullLoggerFactory.Instance);

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task DeleteLeaveAllocation()
        {
            // Arrange
            var command = new DeleteLeaveAllocationCommand
            {
                Id = 1
            };
            
            var handler = new DeleteLeaveAllocationCommandHandler(_mockRepo.Object,_mockAppLogger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<Unit>();
            // Additional assertions
        }
    }
}