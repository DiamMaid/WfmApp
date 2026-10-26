using WfmApp.Domain.Common;

namespace WfmApp.Domain.Events;

public sealed record WarehouseShiftCheckedIn(
    WarehouseShiftId ShiftId,
    DateTimeOffset CheckedInAt,
    string AccessControlEventId) : IDomainEvent;
