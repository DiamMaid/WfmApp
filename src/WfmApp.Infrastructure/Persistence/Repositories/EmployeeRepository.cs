using Microsoft.EntityFrameworkCore;
using WfmApp.Application.Interfaces;
using WfmApp.Domain.Common;
using WfmApp.Domain.Entities;

namespace WfmApp.Infrastructure.Persistence.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly WfmDbContext _context;

    public EmployeeRepository(WfmDbContext context)
    {
        _context = context;
    }

    public async Task<WarehouseEmployee?> GetByIdAsync(EmployeeId id, CancellationToken ct = default)
    {
        return await _context.WarehouseEmployees.FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<IReadOnlyList<WarehouseEmployee>> GetByHubAsync(HubId hubId, CancellationToken ct = default)
    {
        return await _context.WarehouseEmployees
            .Where(e => e.HubId == hubId)
            .OrderBy(e => e.FullName)
            .ToListAsync(ct);
    }

    public async Task AddAsync(WarehouseEmployee employee, CancellationToken ct = default)
    {
        await _context.WarehouseEmployees.AddAsync(employee, ct);
    }
}
