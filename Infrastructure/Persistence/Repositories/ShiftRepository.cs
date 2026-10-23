using Microsoft.EntityFrameworkCore;
using WfmApp.Presentation.Application.Interfaces;
using WfmApp.Presentation.Domain.Common;
using WfmApp.Presentation.Domain.Entities;
using WfmApp.Presentation.Domain.ValueObjects;

namespace WfmApp.Presentation.Infrastructure.Persistence.Repositories;

public class ShiftRepository : IShiftRepository
{
    private readonly WfmDbContext _context;

    public ShiftRepository(WfmDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(WorkShift workShift, CancellationToken ct = default)
    {
        await _context.WorkShifts.AddAsync(workShift, ct);
    }

    public async Task<WorkShift?> GetByIdAsync(WorkShiftId id, CancellationToken ct = default)
    {
        return await _context.WorkShifts.FirstOrDefaultAsync(ws => ws.Id == id, ct);
    }

    public async Task<bool> HasOverlapAsync(EmployeeId employeeId, ShiftInterval interval, CancellationToken ct = default)
    {
        return await _context.WorkShifts
            .AnyAsync(ws => ws.EmployeeId == employeeId
                         && ws.Interval.Start < interval.End
                         && ws.Interval.End > interval.Start, ct);
    }
}
