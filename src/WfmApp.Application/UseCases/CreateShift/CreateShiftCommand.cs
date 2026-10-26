using WfmApp.Domain.Common;
using WfmApp.Domain.Enums;

namespace WfmApp.Application.UseCases.CreateShift;

public sealed class CreateShiftCommand
{
    public required EmployeeId EmployeeId { get; init; }
    public required WorkZone Zone { get; init; }
    public required DateTimeOffset Start { get; init; }
    public required DateTimeOffset End { get; init; }
    public DateTimeOffset? BreakStart { get; init; }
    public TimeSpan? BreakDuration { get; init; }
}
