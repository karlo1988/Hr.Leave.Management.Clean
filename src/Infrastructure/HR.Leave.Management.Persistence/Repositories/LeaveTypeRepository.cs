using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Domain;
using HR.Leave.Management.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HR.Leave.Management.Persistence.Repositories
{
    public class LeaveTypeRepository : GenericRepository<LeaveType>, ILeaveTypeRepository
    {
        public LeaveTypeRepository(HrDatabaseContext context) : base(context)
        {
        }

        public async Task<bool> IsLeaveTypeNameUnique(string name)
        {
            return !await _context.Set<LeaveType>().AnyAsync(lt => lt.Name == name);            
        }

        public async Task<bool> LeaveTypeExists(int id)
        {
            return await _context.Set<LeaveType>().AnyAsync(lt => lt.Id == id);
        }
    }
}