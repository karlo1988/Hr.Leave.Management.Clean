using HR.Leave.Management.Domain;
using HR.Leave.Management.Persistence.DatabaseContext;
using HR.Leave.Management.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace HR.Leave.Management.Persistence.IntegrationTests;

public class LeaveTypeRepositoryTests
{
    private readonly HrDatabaseContext _context;
    private readonly LeaveTypeRepository _repository;

    public LeaveTypeRepositoryTests()
    {
        var dbOptions = new DbContextOptionsBuilder<HrDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new HrDatabaseContext(dbOptions);
        _repository = new LeaveTypeRepository(_context);
    }

    // --- GenericRepository tests ---

    [Fact]
    public async Task AddAsync_ShouldPersistLeaveType()
    {
        var leaveType = new LeaveType { Id = 1, Name = "Vacation", DefaultDays = 10 };

        await _repository.AddAsync(leaveType);

        var result = await _context.LeaveTypes.FindAsync(1);
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Vacation");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectLeaveType()
    {
        var leaveType = new LeaveType { Id = 1, Name = "Vacation", DefaultDays = 10 };
        await _repository.AddAsync(leaveType);

        var result = await _repository.GetByIdAsync(1);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(1);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllLeaveTypes()
    {
        await _repository.AddAsync(new LeaveType { Id = 1, Name = "Vacation", DefaultDays = 10 });
        await _repository.AddAsync(new LeaveType { Id = 2, Name = "Sick", DefaultDays = 5 });

        var results = await _repository.GetAllAsync();

        results.Count.ShouldBe(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        var leaveType = new LeaveType { Id = 1, Name = "Vacation", DefaultDays = 10 };
        await _repository.AddAsync(leaveType);

        leaveType.DefaultDays = 15;
        await _repository.UpdateAsync(leaveType);

        var result = await _repository.GetByIdAsync(1);
        result.DefaultDays.ShouldBe(15);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveLeaveType()
    {
        var leaveType = new LeaveType { Id = 1, Name = "Vacation", DefaultDays = 10 };
        await _repository.AddAsync(leaveType);

        await _repository.DeleteAsync(leaveType);

        var result = await _repository.GetByIdAsync(1);
        result.ShouldBeNull();
    }

    // --- LeaveTypeRepository-specific tests ---

    [Fact]
    public async Task IsLeaveTypeNameUnique_ShouldReturnTrue_WhenNameDoesNotExist()
    {
        var result = await _repository.IsLeaveTypeNameUnique("Maternity");

        result.ShouldBeTrue();
    }

    [Fact]
    public async Task IsLeaveTypeNameUnique_ShouldReturnFalse_WhenNameExists()
    {
        await _repository.AddAsync(new LeaveType { Id = 1, Name = "Sick", DefaultDays = 5 });

        var result = await _repository.IsLeaveTypeNameUnique("Sick");

        result.ShouldBeFalse();
    }

    [Fact]
    public async Task LeaveTypeExists_ShouldReturnTrue_WhenIdExists()
    {
        await _repository.AddAsync(new LeaveType { Id = 1, Name = "Vacation", DefaultDays = 10 });

        var result = await _repository.LeaveTypeExists(1);

        result.ShouldBeTrue();
    }

    [Fact]
    public async Task LeaveTypeExists_ShouldReturnFalse_WhenIdDoesNotExist()
    {
        var result = await _repository.LeaveTypeExists(99);

        result.ShouldBeFalse();
    }
}