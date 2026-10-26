using Microsoft.EntityFrameworkCore;
using WfmApp.Application.Interfaces;
using WfmApp.Domain.Common;
using WfmApp.Domain.Entities;
using WfmApp.Domain.Enums;

namespace WfmApp.Infrastructure.Persistence.Repositories;

public class ShiftRepository : IShiftRepository
{
    private readonly WfmDbContext _context;

    public ShiftRepository(WfmDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(WarehouseShift shift, CancellationToken ct = default)
    {
        await _context.WarehouseShifts.AddAsync(shift, ct);
    }

    public async Task<WarehouseShift?> GetByIdAsync(WarehouseShiftId id, CancellationToken ct = default)
    {
        return await _context.WarehouseShifts.FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<WarehouseShift?> GetPlannedCoveringAsync(
        EmployeeId employeeId,
        DateTimeOffset at,
        CancellationToken ct = default)
    {
        return await _context.WarehouseShifts.FirstOrDefaultAsync(
            s => s.EmployeeId == employeeId
                 && s.Status == ShiftStatus.Planned
                 && s.Planned.Start <= at
                 && at < s.Planned.End,
            ct);
    }

    public async Task<WarehouseShift?> GetActiveByEmployeeIdAsync(EmployeeId employeeId, CancellationToken ct = default)
    {
        return await _context.WarehouseShifts.FirstOrDefaultAsync(
            s => s.EmployeeId == employeeId && s.Status == ShiftStatus.Active,
            ct);
    }

    public async Task<IReadOnlyList<WarehouseShift>> GetActiveByHubAsync(
        HubId hubId,
        DateTimeOffset at,
        CancellationToken ct = default)
    {
        return await _context.WarehouseShifts
            .Where(s => s.HubId == hubId
                        && s.Status == ShiftStatus.Active
                        && s.CheckedInAt <= at
                        && at < s.Planned.End)
            .OrderBy(s => s.Planned.Start)
            .ToListAsync(ct);
    }
}
