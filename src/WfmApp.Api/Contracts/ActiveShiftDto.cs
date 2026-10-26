using WfmApp.Domain.Enums;

namespace WfmApp.Api.Contracts;

public sealed record ActiveShiftDto(
    Guid ShiftId,
    Guid EmployeeId,
    string StaffId,
    string FullName,
    Role Role,
    EmploymentType EmploymentType,
    WorkZone WorkZone,
    DateTimeOffset StartedAt,
    DateTimeOffset PlannedUntil,
    int ElapsedMinutes);
