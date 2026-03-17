using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using HR.Leave.Management.Persistence.DatabaseContext;
using HR.Leave.Management.Application.Contracts.Persistence;
using HR.Leave.Management.Persistence.Repositories;

namespace HR.Leave.Management.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<HrDatabaseContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("HrDatabaseConnectionString")));



        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));    
        services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
        services.AddScoped<ILeaveAllocationRepository, LeaveAllocationRepository>();        
        services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();

        return services;
    }
}