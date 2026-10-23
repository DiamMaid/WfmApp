using WfmApp.Domain.Common;

namespace WfmApp.Domain.Events;

public sealed record WorkShiftConfirmed(WorkShiftId WorkShiftId) : IDomainEvent;