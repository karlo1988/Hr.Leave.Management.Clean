using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HR.Leave.Management.Application.Contracts.Logging;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation;
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
    public class UpdateLeaveAllocationCommandHandlerTests
    {
        private readonly Mock<ILeaveAllocationRepository> _mockRepo;
        private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepo;
        private IMapper _mapper;
        private Mock<IAppLogger<UpdateLeaveAllocationCommandHandler>> _mockAppLogger;
        public UpdateLeaveAllocationCommandHandlerTests()
        {
            _mockRepo = MoqLeaveAllocationRepository.GetLeaveAllocationMoqRepository();
            _mockLeaveTypeRepo = MoqLeaveTypeRepository.GetLeaveTypeMoqRepository();
            _mockAppLogger = new Mock<IAppLogger<UpdateLeaveAllocationCommandHandler>>();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LeaveAllocationProfile>();
                cfg.AddProfile<LeaveTypeProfile>();
            }, NullLoggerFactory.Instance);

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task UpdateLeaveAllocation()
        {
            // Arrange
            var command = new UpdateLeaveAllocationCommand
            {
                Id = 1,
                EmployeeId = "1",
                LeaveTypeId = 1,
                NumberOfDays = 5,                            
                Period = DateTime.Now.Year
            };
            
            var handler = new UpdateLeaveAllocationCommandHandler(_mockRepo.Object, _mockLeaveTypeRepo.Object, _mapper, _mockAppLogger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<Unit>();
            // Additional assertions
        }
    }
}