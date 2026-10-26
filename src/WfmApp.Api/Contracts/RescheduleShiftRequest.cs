namespace WfmApp.Api.Contracts;

public sealed class RescheduleShiftRequest
{
    public required DateTimeOffset Start { get; init; }
    public required DateTimeOffset End { get; init; }
    public DateTimeOffset? BreakStart { get; init; }
    public int? BreakMinutes { get; init; }
}
