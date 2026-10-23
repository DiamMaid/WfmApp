using WfmApp.Domain.Common;
using WfmApp.Domain.Entities;
using WfmApp.Domain.ValueObjects;

namespace WfmApp.Application.Interfaces;

public interface IShiftRepository
{
    Task AddAsync(WorkShift workShift, CancellationToken ct = default);
    Task<WorkShift?> GetByIdAsync(WorkShiftId id, CancellationToken ct = default);
    Task<bool> HasOverlapAsync(EmployeeId employeeId, ShiftInterval interval, CancellationToken ct = default);
}