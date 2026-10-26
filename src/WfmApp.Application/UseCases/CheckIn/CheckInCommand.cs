using WfmApp.Domain.Common;

namespace WfmApp.Application.UseCases.CheckIn;

public sealed class CheckInCommand
{
    public required EmployeeId EmployeeId { get; init; }
    public required HubId HubId { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required string AccessControlEventId { get; init; }
}
