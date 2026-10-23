using WfmApp.Presentation.Domain.Common;
using WfmApp.Presentation.Domain.ValueObjects;

namespace WfmApp.Presentation.Domain.Events;

public sealed record WorkShiftRescheduled(
    WorkShiftId WorkShiftId,
    ShiftInterval NewInterval) : IDomainEvent;