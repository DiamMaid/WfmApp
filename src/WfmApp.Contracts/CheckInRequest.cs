namespace WfmApp.Contracts;

public sealed class CheckInRequest
{
    public required Guid EmployeeId { get; init; }
    public required Guid HubId { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required string AccessControlEventId { get; init; }
}
