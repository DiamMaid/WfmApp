using WfmApp.Domain.Common;
using WfmApp.Domain.Enums;

namespace WfmApp.Domain.Events;

public sealed record WarehouseEmployeeCreated(
    EmployeeId EmployeeId,
    string StaffId,
    string FullName,
    HubId HubId,
    Role Role,
    EmploymentType EmploymentType) : IDomainEvent;
