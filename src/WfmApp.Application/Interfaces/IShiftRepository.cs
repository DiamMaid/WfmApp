using WfmApp.Domain.Common;
using WfmApp.Domain.Entities;

namespace WfmApp.Application.Interfaces;

public interface IShiftRepository
{
    Task AddAsync(WarehouseShift shift, CancellationToken ct = default);
    Task<WarehouseShift?> GetByIdAsync(WarehouseShiftId id, CancellationToken ct = default);
    Task<WarehouseShift?> GetPlannedCoveringAsync(EmployeeId employeeId, DateTimeOffset at, CancellationToken ct = default);
    Task<WarehouseShift?> GetActiveByEmployeeIdAsync(EmployeeId employeeId, CancellationToken ct = default);
    Task<IReadOnlyList<WarehouseShift>> GetActiveByHubAsync(HubId hubId, DateTimeOffset at, CancellationToken ct = default);
    Task<WarehouseShift?> GetByAccessControlEventIdAsync(string accessControlEventId, CancellationToken ct = default);
}
