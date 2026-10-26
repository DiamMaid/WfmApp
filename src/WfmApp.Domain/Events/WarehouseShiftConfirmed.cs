using WfmApp.Domain.Common;

namespace WfmApp.Domain.Events;

public sealed record WarehouseShiftConfirmed(WarehouseShiftId ShiftId) : IDomainEvent;
