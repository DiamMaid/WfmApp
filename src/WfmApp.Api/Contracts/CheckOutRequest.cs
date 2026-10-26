namespace WfmApp.Api.Contracts;

public sealed class CheckOutRequest
{
    public required Guid ShiftId { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public int? UnitsProduced { get; init; }
}
