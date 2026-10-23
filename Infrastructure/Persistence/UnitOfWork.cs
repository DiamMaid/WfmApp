using WfmApp.Presentation.Application.Interfaces;

namespace WfmApp.Presentation.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly WfmDbContext _context;

    public UnitOfWork(WfmDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
}
