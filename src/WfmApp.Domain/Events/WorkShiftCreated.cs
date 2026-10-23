using WfmApp.Domain.Common;
using WfmApp.Domain.ValueObjects;

namespace WfmApp.Domain.Events;

public sealed record WorkShiftCreated(
    WorkShiftId WorkShiftId,
    EmployeeId EmployeeId,
    ShiftInterval Interval) : IDomainEvent;