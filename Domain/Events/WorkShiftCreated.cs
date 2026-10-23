using WfmApp.Presentation.Domain.Common;
using WfmApp.Presentation.Domain.ValueObjects;

namespace WfmApp.Presentation.Domain.Events;

public sealed record WorkShiftCreated(
    WorkShiftId WorkShiftId,
    EmployeeId EmployeeId,
    ShiftInterval Interval) : IDomainEvent;