using Microsoft.EntityFrameworkCore;
using WfmApp.Application.Interfaces;
using WfmApp.Domain.Common;
using WfmApp.Domain.Entities;

namespace WfmApp.Infrastructure.Persistence.Repositories;

public class HubRepository : IHubRepository
{
    private readonly WfmDbContext _context;

    public HubRepository(WfmDbContext context)
    {
        _context = context;
    }

    public async Task<WarehouseHub?> GetByIdAsync(HubId id, CancellationToken ct = default)
    {
        return await _context.WarehouseHubs.FirstOrDefaultAsync(h => h.Id == id, ct);
    }

    public async Task<IReadOnlyList<WarehouseHub>> GetActiveAsync(CancellationToken ct = default)
    {
        return await _context.WarehouseHubs
            .Where(h => h.IsActive)
            .OrderBy(h => h.Location.City)
            .ThenBy(h => h.Code)
            .ToListAsync(ct);
    }

    public async Task AddAsync(WarehouseHub hub, CancellationToken ct = default)
    {
        await _context.WarehouseHubs.AddAsync(hub, ct);
    }
}
