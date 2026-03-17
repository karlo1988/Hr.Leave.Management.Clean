using HR.Leave.Management.Domain;

namespace HR.Leave.Management.Application.Contracts.Persistence;

public interface ILeaveTypeRepository : IGenericRepository<LeaveType>
{    
    Task<bool> IsLeaveTypeNameUnique(string name);
    Task<bool> LeaveTypeExists(int id);
}
