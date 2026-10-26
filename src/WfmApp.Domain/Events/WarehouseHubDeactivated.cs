using WfmApp.Domain.Common;

namespace WfmApp.Domain.Events;

public sealed record WarehouseHubDeactivated(HubId HubId) : IDomainEvent;
