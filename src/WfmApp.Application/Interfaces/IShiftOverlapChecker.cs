using WfmApp.Domain.Common;
using WfmApp.Domain.ValueObjects;

namespace WfmApp.Application.Interfaces;

public interface IShiftOverlapChecker
{
    Task<bool> HasOverlapAsync(
        EmployeeId employeeId,
        ShiftInterval interval,
        WarehouseShiftId? excludeShiftId = null,
        CancellationToken ct = default);
}
