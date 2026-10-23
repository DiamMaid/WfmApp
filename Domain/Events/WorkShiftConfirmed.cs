using WfmApp.Presentation.Domain.Common;

namespace WfmApp.Presentation.Domain.Events;

public sealed record WorkShiftConfirmed(WorkShiftId WorkShiftId) : IDomainEvent;