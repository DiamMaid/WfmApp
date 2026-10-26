using Microsoft.EntityFrameworkCore;
using WfmApp.Domain.Entities;

namespace WfmApp.Infrastructure.Persistence;

public class WfmDbContext : DbContext
{
    public WfmDbContext(DbContextOptions<WfmDbContext> options) : base(options)
    {
    }

    public DbSet<WarehouseHub> WarehouseHubs => Set<WarehouseHub>();
    public DbSet<WarehouseEmployee> WarehouseEmployees => Set<WarehouseEmployee>();
    public DbSet<WarehouseShift> WarehouseShifts => Set<WarehouseShift>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("btree_gist");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WfmDbContext).Assembly);
    }
}
