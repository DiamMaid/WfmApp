using Microsoft.EntityFrameworkCore;
using WfmApp.Presentation.Domain.Entities;

namespace WfmApp.Presentation.Infrastructure.Persistence;

public class WfmDbContext : DbContext
{
    public WfmDbContext(DbContextOptions<WfmDbContext> options) : base(options)
    {
    }

    public DbSet<WorkShift> WorkShifts => Set<WorkShift>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("btree_gist");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WfmDbContext).Assembly);
    }
}
