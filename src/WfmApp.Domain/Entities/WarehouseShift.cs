using WfmApp.Domain.Common;
using WfmApp.Domain.Enums;
using WfmApp.Domain.Events;
using WfmApp.Domain.ValueObjects;

namespace WfmApp.Domain.Entities;

public sealed class WarehouseShift : AggregateRoot<WarehouseShiftId>
{
    private EmployeeId _employeeId;
    private HubId _hubId;
    private WorkZone _zone;
    private ShiftInterval _planned;
    private DateTimeOffset? _checkedInAt;
    private DateTimeOffset? _checkedOutAt;
    private ShiftStatus _status;
    private bool _isConfirmed;
    private string? _accessControlEventId;

    private WarehouseShift()
    {
        _planned = null!;
    }

    private WarehouseShift(
        WarehouseShiftId id,
        EmployeeId employeeId,
        HubId hubId,
        WorkZone zone,
        ShiftInterval planned)
        : base(id)
    {
        _employeeId = employeeId;
        _hubId = hubId;
        _zone = zone;
        _planned = planned;
        _status = ShiftStatus.Planned;
    }

    public EmployeeId EmployeeId => _employeeId;
    public HubId HubId => _hubId;
    public WorkZone Zone => _zone;
    public ShiftInterval Planned => _planned;
    public DateTimeOffset? CheckedInAt => _checkedInAt;
    public DateTimeOffset? CheckedOutAt => _checkedOutAt;
    public ShiftStatus Status => _status;
    public bool IsConfirmed => _isConfirmed;
    public string? AccessControlEventId => _accessControlEventId;

    public TimeSpan WorkedDuration
    {
        get
        {
            if (_status != ShiftStatus.Closed || _checkedInAt is null || _checkedOutAt is null)
                return _planned.Worked;

            return _checkedOutAt.Value - _checkedInAt.Value - (_planned.Break?.Duration ?? TimeSpan.Zero);
        }
    }

    public static WarehouseShift Create(
        EmployeeId employeeId,
        HubId hubId,
        WorkZone zone,
        ShiftInterval planned)
    {
        if (employeeId == default)
            throw new ArgumentException("EmployeeId must be set.", nameof(employeeId));

        if (hubId == default)
            throw new ArgumentException("HubId must be set.", nameof(hubId));

        var shift = new WarehouseShift(WarehouseShiftId.New(), employeeId, hubId, zone, planned);
        shift.AddDomainEvent(new WarehouseShiftCreated(shift.Id, employeeId, hubId, zone, planned));

        return shift;
    }

    public void CheckIn(DateTimeOffset at, string accessControlEventId)
    {
        if (_status != ShiftStatus.Planned)
            throw new InvalidOperationException("Only a planned shift can be checked in.");

        if (at == default)
            throw new ArgumentException("Check-in time must be set.", nameof(at));

        if (string.IsNullOrWhiteSpace(accessControlEventId))
            throw new ArgumentException("Access control event id must not be empty.", nameof(accessControlEventId));

        _checkedInAt = at;
        _accessControlEventId = accessControlEventId;
        _status = ShiftStatus.Active;

        AddDomainEvent(new WarehouseShiftCheckedIn(Id, at, accessControlEventId));
    }

    public void CheckOut(DateTimeOffset at)
    {
        if (_status != ShiftStatus.Active)
            throw new InvalidOperationException("Only an active shift can be checked out.");

        if (_checkedInAt is null)
            throw new InvalidOperationException("An active shift always has a check-in time.");

        if (at <= _checkedInAt.Value)
            throw new ArgumentException("Check-out time must be after check-in time.", nameof(at));

        _checkedOutAt = at;
        _status = ShiftStatus.Closed;

        AddDomainEvent(new WarehouseShiftCheckedOut(Id, at, WorkedDuration));
    }

    public void Reschedule(ShiftInterval planned)
    {
        ThrowIfClosed();

        if (_isConfirmed)
            throw new InvalidOperationException("A confirmed shift cannot be rescheduled.");

        _planned = planned;

        AddDomainEvent(new WarehouseShiftRescheduled(Id, planned));
    }

    public void Confirm()
    {
        ThrowIfClosed();

        if (_isConfirmed)
            return;

        _isConfirmed = true;

        AddDomainEvent(new WarehouseShiftConfirmed(Id));
    }

    private void ThrowIfClosed()
    {
        if (_status == ShiftStatus.Closed)
            throw new InvalidOperationException("A closed shift is immutable.");
    }
}
