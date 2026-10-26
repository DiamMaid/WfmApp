using WfmApp.Domain.Common;
using WfmApp.Domain.ValueObjects;

namespace WfmApp.Domain.Events;

public sealed record WarehouseShiftRescheduled(
    WarehouseShiftId ShiftId,
    ShiftInterval Planned) : IDomainEvent;
