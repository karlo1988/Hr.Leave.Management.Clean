using HR.Leave.Management.Domain;
using HR.Leave.Management.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Shouldly;


namespace HR.Leave.Management.Persistence.IntegrationTests
{

    public class HrDatabaseContextTests
    {
        private readonly HrDatabaseContext _context;
        public HrDatabaseContextTests()
        {
            var dbOptions = new DbContextOptionsBuilder<HrDatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new HrDatabaseContext(dbOptions);
        }
        
        [Fact]
        public async Task Save_SetDateCreatedValue()
        {

            //Arrange
            var leaveType = new LeaveType
            {
                Id = 1,
                Name = "Test Vacation",
                DefaultDays = 10
            };

            //Act
            await _context.LeaveTypes.AddAsync(leaveType);
            await _context.SaveChangesAsync();

            //Assert
            leaveType.CreatedDate.Date.ShouldBe(DateTime.UtcNow.Date);
        

        }

        [Fact]
        public async Task Save_SetDateModifiedValue()
        {
            //Arrange
            var leaveType = new LeaveType
            {
                Id = 1,
                Name = "Test Vacation",
                DefaultDays = 10
            };

            await _context.LeaveTypes.AddAsync(leaveType);
            await _context.SaveChangesAsync();

            //Act
            leaveType.Name = "Test Vacation Updated";
            _context.LeaveTypes.Update(leaveType);
            await _context.SaveChangesAsync();

            //Assert
            leaveType.ModifiedDate.ShouldNotBeNull();
            leaveType.ModifiedDate?.Date.ShouldBe(DateTime.UtcNow.Date);

        }
    }
}
