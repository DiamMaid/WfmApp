using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WfmApp.Infrastructure.Persistence;

public sealed class WfmDbContextFactory : IDesignTimeDbContextFactory<WfmDbContext>
{
    public WfmDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<WfmDbContext>()
            .UseNpgsql(WfmDatabase.ConnectionString)
            .Options;

        return new WfmDbContext(options);
    }
}
