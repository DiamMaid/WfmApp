using WfmApp.Domain.Common;
using WfmApp.Domain.Enums;
using WfmApp.Domain.ValueObjects;

namespace WfmApp.Domain.Events;

public sealed record WarehouseShiftCreated(
    WarehouseShiftId ShiftId,
    EmployeeId EmployeeId,
    HubId HubId,
    WorkZone Zone,
    ShiftInterval Planned) : IDomainEvent;
