using WfmApp.Domain.Enums;

namespace WfmApp.Api.Contracts;

public sealed record ShiftDto(
    Guid ShiftId,
    Guid EmployeeId,
    Guid HubId,
    WorkZone Zone,
    DateTimeOffset PlannedStart,
    DateTimeOffset PlannedEnd,
    DateTimeOffset? BreakStart,
    int? BreakMinutes,
    DateTimeOffset? CheckedInAt,
    DateTimeOffset? CheckedOutAt,
    ShiftStatus Status,
    bool IsConfirmed,
    int WorkedMinutes);
