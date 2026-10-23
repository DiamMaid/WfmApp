using WfmApp.Domain.Common;
using WfmApp.Domain.ValueObjects;

namespace WfmApp.Domain.Events;

public sealed record WorkShiftRescheduled(
    WorkShiftId WorkShiftId,
    ShiftInterval NewInterval) : IDomainEvent;