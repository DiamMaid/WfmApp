using WfmApp.Domain.Common;

namespace WfmApp.Domain.Events;

public sealed record WarehouseHubActivated(HubId HubId) : IDomainEvent;
