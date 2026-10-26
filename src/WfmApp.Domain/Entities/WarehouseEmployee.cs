using WfmApp.Domain.Common;
using WfmApp.Domain.Enums;
using WfmApp.Domain.Events;

namespace WfmApp.Domain.Entities;

public sealed class WarehouseEmployee : AggregateRoot<EmployeeId>
{
    private string _staffId;
    private string _fullName;
    private HubId _hubId;
    private Role _role;
    private EmploymentType _employmentType;
    private EmploymentStatus _status;

    private WarehouseEmployee()
    {
        _staffId = null!;
        _fullName = null!;
    }

    private WarehouseEmployee(
        EmployeeId id,
        string staffId,
        string fullName,
        HubId hubId,
        Role role,
        EmploymentType employmentType)
        : base(id)
    {
        _staffId = staffId;
        _fullName = fullName;
        _hubId = hubId;
        _role = role;
        _employmentType = employmentType;
        _status = EmploymentStatus.Active;
    }

    public string StaffId => _staffId;
    public string FullName => _fullName;
    public HubId HubId => _hubId;
    public Role Role => _role;
    public EmploymentType EmploymentType => _employmentType;
    public EmploymentStatus Status => _status;

    public static WarehouseEmployee Create(
        string staffId,
        string fullName,
        HubId hubId,
        Role role,
        EmploymentType employmentType)
    {
        ValidateStaffId(staffId);
        ValidateFullName(fullName);

        if (hubId == default)
            throw new ArgumentException("HubId must be set.", nameof(hubId));

        var employee = new WarehouseEmployee(EmployeeId.New(), staffId, fullName, hubId, role, employmentType);
        employee.AddDomainEvent(new WarehouseEmployeeCreated(
            employee.Id, staffId, fullName, hubId, role, employmentType));

        return employee;
    }

    public void AssignToHub(HubId hubId)
    {
        if (hubId == default)
            throw new ArgumentException("HubId must be set.", nameof(hubId));

        if (hubId == _hubId)
            return;

        _hubId = hubId;
        AddDomainEvent(new WarehouseEmployeeAssignedToHub(Id, hubId));
    }

    public void ChangeRole(Role role)
    {
        _role = role;
    }

    public void ChangeEmploymentType(EmploymentType employmentType)
    {
        _employmentType = employmentType;
    }

    public void SendOnVacation()
    {
        if (_status == EmploymentStatus.Vacation)
            throw new InvalidOperationException("Employee is already on vacation.");

        _status = EmploymentStatus.Vacation;
    }

    public void Reinstate()
    {
        if (_status != EmploymentStatus.Vacation)
            throw new InvalidOperationException("Only an employee on vacation can be reinstated.");

        _status = EmploymentStatus.Active;
    }

    private static void ValidateStaffId(string staffId)
    {
        if (string.IsNullOrWhiteSpace(staffId))
            throw new ArgumentException("StaffId must not be empty.", nameof(staffId));
    }

    private static void ValidateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName must not be empty.", nameof(fullName));
    }
}
