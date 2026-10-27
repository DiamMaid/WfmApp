using WfmApp.Domain.Enums;

namespace WfmApp.Contracts;

public sealed record EmployeeDto(
    Guid Id,
    string StaffId,
    string FullName,
    Guid HubId,
    Role Role,
    EmploymentType EmploymentType,
    EmploymentStatus Status);
