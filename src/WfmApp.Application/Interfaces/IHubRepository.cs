using WfmApp.Domain.Common;
using WfmApp.Domain.Entities;

namespace WfmApp.Application.Interfaces;

public interface IHubRepository
{
    Task<WarehouseHub?> GetByIdAsync(HubId id, CancellationToken ct = default);
    Task<IReadOnlyList<WarehouseHub>> GetActiveAsync(CancellationToken ct = default);
    Task AddAsync(WarehouseHub hub, CancellationToken ct = default);
}
