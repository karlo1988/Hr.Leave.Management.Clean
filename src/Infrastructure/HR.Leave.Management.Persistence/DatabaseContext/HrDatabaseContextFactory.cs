using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HR.Leave.Management.Persistence.DatabaseContext
{
    public class HrDatabaseContextFactory : IDesignTimeDbContextFactory<HrDatabaseContext>
    {
        public HrDatabaseContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<HrDatabaseContext>();
            builder.UseNpgsql(configuration.GetConnectionString("HrDatabaseConnectionString"));

            return new HrDatabaseContext(builder.Options);
        }
    }
}
