using Microsoft.EntityFrameworkCore;
using WfmApp.Application.Interfaces;
using WfmApp.Domain.Common;
using WfmApp.Domain.ValueObjects;

namespace WfmApp.Infrastructure.Persistence.Repositories;

public class ShiftOverlapChecker : IShiftOverlapChecker
{
    private readonly WfmDbContext _context;

    public ShiftOverlapChecker(WfmDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasOverlapAsync(
        EmployeeId employeeId,
        ShiftInterval interval,
        WarehouseShiftId? excludeShiftId = null,
        CancellationToken ct = default)
    {
        var query = _context.WarehouseShifts.Where(s =>
            s.EmployeeId == employeeId
            && s.Planned.Start < interval.End
            && s.Planned.End > interval.Start);

        if (excludeShiftId is { } excluded)
            query = query.Where(s => s.Id != excluded);

        return await query.AnyAsync(ct);
    }
}
