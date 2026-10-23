using WfmApp.Presentation.Domain.Common;
using WfmApp.Presentation.Domain.Entities;
using WfmApp.Presentation.Domain.ValueObjects;

namespace WfmApp.Presentation.Application.Interfaces;

public interface IShiftRepository
{
    Task AddAsync(WorkShift workShift, CancellationToken ct = default);
    Task<WorkShift?> GetByIdAsync(WorkShiftId id, CancellationToken ct = default);
    Task<bool> HasOverlapAsync(EmployeeId employeeId, ShiftInterval interval, CancellationToken ct = default);
}