using WfmApp.Domain.Common;

namespace WfmApp.Domain.Events;

public sealed record WarehouseShiftCheckedOut(
    WarehouseShiftId ShiftId,
    DateTimeOffset CheckedOutAt,
    TimeSpan WorkedDuration) : IDomainEvent;
