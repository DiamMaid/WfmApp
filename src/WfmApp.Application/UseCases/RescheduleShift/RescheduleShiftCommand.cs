using WfmApp.Domain.Common;

namespace WfmApp.Application.UseCases.RescheduleShift;

public sealed class RescheduleShiftCommand
{
    public required WarehouseShiftId ShiftId { get; init; }
    public required DateTimeOffset Start { get; init; }
    public required DateTimeOffset End { get; init; }
    public DateTimeOffset? BreakStart { get; init; }
    public TimeSpan? BreakDuration { get; init; }
}
