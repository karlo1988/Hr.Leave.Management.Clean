using HR.Leave.Management.Domain;
using HR.Leave.Management.Persistence.DatabaseContext;
using HR.Leave.Management.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace HR.Leave.Management.Persistence.IntegrationTests;

public class LeaveAllocationRepositoryTests
{
    private readonly HrDatabaseContext _context;
    private readonly LeaveAllocationRepository _repository;

    public LeaveAllocationRepositoryTests()
    {
        var dbOptions = new DbContextOptionsBuilder<HrDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new HrDatabaseContext(dbOptions);
        _repository = new LeaveAllocationRepository(_context);
    }

    private async Task<LeaveType> SeedLeaveTypeAsync(int id = 1)
    {
        var leaveType = new LeaveType { Id = id, Name = "Vacation", DefaultDays = 10 };
        await _context.LeaveTypes.AddAsync(leaveType);
        await _context.SaveChangesAsync();
        return leaveType;
    }

    private LeaveAllocation BuildAllocation(int id, int leaveTypeId, string employeeId = "emp-1", int period = 2025) =>
        new LeaveAllocation
        {
            Id = id,
            LeaveTypeId = leaveTypeId,
            LeaveType = null!,
            EmployeeId = employeeId,
            NumberOfDays = 10,
            Period = period
        };

    // --- GenericRepository tests ---

    [Fact]
    public async Task AddAsync_ShouldPersistLeaveAllocation()
    {
        await SeedLeaveTypeAsync();
        var allocation = BuildAllocation(1, 1);

        await _repository.AddAsync(allocation);

        var result = await _context.LeaveAllocations.FindAsync(1);
        result.ShouldNotBeNull();
        result.EmployeeId.ShouldBe("emp-1");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectAllocation()
    {
        await SeedLeaveTypeAsync();
        await _repository.AddAsync(BuildAllocation(1, 1));

        var result = await _repository.GetByIdAsync(1);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(1);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllAllocations()
    {
        await SeedLeaveTypeAsync();
        await _repository.AddAsync(BuildAllocation(1, 1, "emp-1"));
        await _repository.AddAsync(BuildAllocation(2, 1, "emp-2"));

        var results = await _repository.GetAllAsync();

        results.Count.ShouldBe(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        await SeedLeaveTypeAsync();
        var allocation = BuildAllocation(1, 1);
        await _repository.AddAsync(allocation);

        allocation.NumberOfDays = 20;
        await _repository.UpdateAsync(allocation);

        var result = await _repository.GetByIdAsync(1);
        result.NumberOfDays.ShouldBe(20);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveAllocation()
    {
        await SeedLeaveTypeAsync();
        var allocation = BuildAllocation(1, 1);
        await _repository.AddAsync(allocation);

        await _repository.DeleteAsync(allocation);

        var result = await _repository.GetByIdAsync(1);
        result.ShouldBeNull();
    }

    // --- LeaveAllocationRepository-specific tests ---

    [Fact]
    public async Task AddAllocations_ShouldPersistMultipleAllocations()
    {
        await SeedLeaveTypeAsync();
        var allocations = new List<LeaveAllocation>
        {
            BuildAllocation(1, 1, "emp-1"),
            BuildAllocation(2, 1, "emp-2")
        };

        await _repository.AddAllocations(allocations);

        var results = await _context.LeaveAllocations.ToListAsync();
        results.Count.ShouldBe(2);
    }

    [Fact]
    public async Task AllocationExists_ShouldReturnTrue_WhenMatchFound()
    {
        await SeedLeaveTypeAsync();
        await _repository.AddAsync(BuildAllocation(1, 1, "emp-1", 2025));

        var result = await _repository.AllocationExists("emp-1", 1, 2025);

        result.ShouldBeTrue();
    }

    [Fact]
    public async Task AllocationExists_ShouldReturnFalse_WhenNoMatch()
    {
        await SeedLeaveTypeAsync();
        await _repository.AddAsync(BuildAllocation(1, 1, "emp-1", 2025));

        var result = await _repository.AllocationExists("emp-1", 1, 2024);

        result.ShouldBeFalse();
    }

    [Fact]
    public async Task GetLeaveAllocationsWithDetails_ShouldReturnAllWithLeaveType()
    {
        await SeedLeaveTypeAsync();
        await _repository.AddAsync(BuildAllocation(1, 1, "emp-1"));
        await _repository.AddAsync(BuildAllocation(2, 1, "emp-2"));

        var results = await _repository.GetLeaveAllocationsWithDetails();

        results.Count.ShouldBe(2);
        results.ShouldAllBe(a => a.LeaveType != null);
    }

    [Fact]
    public async Task GetLeaveAllocationsWithDetails_ByUserId_ShouldReturnOnlyMatchingAllocations()
    {
        await SeedLeaveTypeAsync();
        await _repository.AddAsync(BuildAllocation(1, 1, "emp-1"));
        await _repository.AddAsync(BuildAllocation(2, 1, "emp-2"));

        var results = await _repository.GetLeaveAllocationsWithDetails("emp-1");

        results.Count.ShouldBe(1);
        results[0].EmployeeId.ShouldBe("emp-1");
    }

    [Fact]
    public async Task GetLeaveAllocationWithDetails_ShouldIncludeLeaveType()
    {
        await SeedLeaveTypeAsync();
        await _repository.AddAsync(BuildAllocation(1, 1));

        var result = await _repository.GetLeaveAllocationWithDetails(1);

        result.ShouldNotBeNull();
        result.LeaveType.ShouldNotBeNull();
        result.LeaveType.Name.ShouldBe("Vacation");
    }

    [Fact]
    public async Task GetLeaveAllocationWithDetails_ShouldReturnNull_WhenNotFound()
    {
        var result = await _repository.GetLeaveAllocationWithDetails(99);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetUserAllocations_ShouldReturnCorrectAllocation()
    {
        await SeedLeaveTypeAsync();
        await _repository.AddAsync(BuildAllocation(1, 1, "emp-1"));

        var result = await _repository.GetUserAllocations("emp-1", 1);

        result.ShouldNotBeNull();
        result.EmployeeId.ShouldBe("emp-1");
        result.LeaveTypeId.ShouldBe(1);
    }

    [Fact]
    public async Task GetUserAllocations_ShouldReturnNull_WhenNoMatch()
    {
        await SeedLeaveTypeAsync();

        var result = await _repository.GetUserAllocations("emp-999", 1);

        result.ShouldBeNull();
    }
}
