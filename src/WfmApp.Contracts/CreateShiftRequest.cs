using WfmApp.Domain.Enums;

namespace WfmApp.Contracts;

public sealed class CreateShiftRequest
{
    public required Guid EmployeeId { get; init; }
    public required WorkZone Zone { get; init; }
    public required DateTimeOffset Start { get; init; }
    public required DateTimeOffset End { get; init; }
    public DateTimeOffset? BreakStart { get; init; }
    public int? BreakMinutes { get; init; }
}
