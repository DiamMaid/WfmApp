using WfmApp.Domain.Common;
using WfmApp.Domain.ValueObjects;

namespace WfmApp.Domain.Events;

public sealed record WarehouseHubCreated(
    HubId HubId,
    string Code,
    string Name,
    HubLocation Location,
    string TimeZone) : IDomainEvent;
