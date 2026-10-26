using WfmApp.Domain.Common;
using WfmApp.Domain.Entities;

namespace WfmApp.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<WarehouseEmployee?> GetByIdAsync(EmployeeId id, CancellationToken ct = default);
    Task<IReadOnlyList<WarehouseEmployee>> GetByHubAsync(HubId hubId, CancellationToken ct = default);
    Task AddAsync(WarehouseEmployee employee, CancellationToken ct = default);
}
