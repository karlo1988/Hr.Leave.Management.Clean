using HR.Leave.Management.Domain;
using HR.Leave.Management.Persistence.DatabaseContext;
using HR.Leave.Management.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace HR.Leave.Management.Persistence.IntegrationTests;

public class LeaveRequestRepositoryTests
{
    private readonly HrDatabaseContext _context;
    private readonly LeaveRequestRepository _repository;

    public LeaveRequestRepositoryTests()
    {
        var dbOptions = new DbContextOptionsBuilder<HrDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new HrDatabaseContext(dbOptions);
        _repository = new LeaveRequestRepository(_context);
    }

    private async Task<LeaveType> SeedLeaveTypeAsync(int id = 1)
    {
        var leaveType = new LeaveType { Id = id, Name = "Vacation", DefaultDays = 10 };
        await _context.LeaveTypes.AddAsync(leaveType);
        await _context.SaveChangesAsync();
        return leaveType;
    }

    private LeaveRequest BuildLeaveRequest(int id, int leaveTypeId, string employeeId = "emp-1") =>
        new LeaveRequest
        {
            Id = id,
            LeaveTypeId = leaveTypeId,
            LeaveType = null!,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(3),
            DateRequested = DateTime.UtcNow,
            RequestingEmployeeId = employeeId
        };

    // --- GenericRepository tests ---

    [Fact]
    public async Task AddAsync_ShouldPersistLeaveRequest()
    {
        await SeedLeaveTypeAsync();
        var request = BuildLeaveRequest(1, 1);

        await _repository.AddAsync(request);

        var result = await _context.LeaveRequests.FindAsync(1);
        result.ShouldNotBeNull();
        result.RequestingEmployeeId.ShouldBe("emp-1");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectLeaveRequest()
    {
        await SeedLeaveTypeAsync();
        await _repository.AddAsync(BuildLeaveRequest(1, 1));

        var result = await _repository.GetByIdAsync(1);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(1);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllLeaveRequests()
    {
        await SeedLeaveTypeAsync();
        await _repository.AddAsync(BuildLeaveRequest(1, 1, "emp-1"));
        await _repository.AddAsync(BuildLeaveRequest(2, 1, "emp-2"));

        var results = await _repository.GetAllAsync();

        results.Count.ShouldBe(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        await SeedLeaveTypeAsync();
        var request = BuildLeaveRequest(1, 1);
        await _repository.AddAsync(request);

        request.Approved = true;
        await _repository.UpdateAsync(request);

        var result = await _repository.GetByIdAsync(1);
        result.Approved.ShouldBe(true);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveLeaveRequest()
    {
        await SeedLeaveTypeAsync();
        var request = BuildLeaveRequest(1, 1);
        await _repository.AddAsync(request);

        await _repository.DeleteAsync(request);

        var result = await _repository.GetByIdAsync(1);
        result.ShouldBeNull();
    }

    // --- LeaveRequestRepository-specific tests ---

    [Fact]
    public async Task GetLeaveRequestWithDetails_ShouldIncludeLeaveType()
    {
        await SeedLeaveTypeAsync(id: 1);
        await _repository.AddAsync(BuildLeaveRequest(1, 1));

        var result = await _repository.GetLeaveRequestWithDetails(1);

        result.ShouldNotBeNull();
        result.LeaveType.ShouldNotBeNull();
        result.LeaveType.Name.ShouldBe("Vacation");
    }

    [Fact]
    public async Task GetLeaveRequestWithDetails_ShouldReturnNull_WhenNotFound()
    {
        var result = await _repository.GetLeaveRequestWithDetails(99);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetLeaveRequestsWithDetails_ShouldReturnAllWithLeaveType()
    {
        await SeedLeaveTypeAsync();
        await _repository.AddAsync(BuildLeaveRequest(1, 1, "emp-1"));
        await _repository.AddAsync(BuildLeaveRequest(2, 1, "emp-2"));

        var results = await _repository.GetLeaveRequestsWithDetails();

        results.Count.ShouldBe(2);
        results.ShouldAllBe(r => r.LeaveType != null);
    }

    [Fact]
    public async Task GetLeaveRequestsWithDetails_ByUserId_ShouldReturnOnlyMatchingRequests()
    {
        await SeedLeaveTypeAsync();
        await _repository.AddAsync(BuildLeaveRequest(1, 1, "emp-1"));
        await _repository.AddAsync(BuildLeaveRequest(2, 1, "emp-2"));

        var results = await _repository.GetLeaveRequestsWithDetails("emp-1");

        results.Count.ShouldBe(1);
        results[0].RequestingEmployeeId.ShouldBe("emp-1");
    }

    [Fact]
    public async Task GetLeaveRequestsWithDetails_ByUserId_ShouldReturnEmpty_WhenNoMatch()
    {
        await SeedLeaveTypeAsync();
        await _repository.AddAsync(BuildLeaveRequest(1, 1, "emp-1"));

        var results = await _repository.GetLeaveRequestsWithDetails("emp-999");

        results.ShouldBeEmpty();
    }
}
