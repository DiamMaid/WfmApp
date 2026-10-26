using WfmApp.Domain.Common;

namespace WfmApp.Domain.Events;

public sealed record WarehouseEmployeeAssignedToHub(
    EmployeeId EmployeeId,
    HubId HubId) : IDomainEvent;
