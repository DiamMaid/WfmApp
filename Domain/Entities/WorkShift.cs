using WfmApp.Presentation.Domain.Common;
using WfmApp.Presentation.Domain.Events;
using WfmApp.Presentation.Domain.ValueObjects;

namespace WfmApp.Presentation.Domain.Entities;

public sealed class WorkShift : AggregateRoot<WorkShiftId>
{
    private EmployeeId _employeeId;
    private ShiftInterval _interval;
    private bool _isConfirmed;

    private WorkShift()
    {
        _interval = null!;
    }

    private WorkShift(WorkShiftId id, EmployeeId employeeId, ShiftInterval interval) : base(id)
    {
        _employeeId = employeeId;
        _interval = interval;
        _isConfirmed = false;
    }

    public EmployeeId EmployeeId => _employeeId;
    public ShiftInterval Interval => _interval;
    public bool IsConfirmed => _isConfirmed;

    public static WorkShift Create(EmployeeId employeeId, ShiftInterval interval)
    {
        if (employeeId == default)
            throw new ArgumentException("EmployeeId must be set.", nameof(employeeId));

        var workShift = new WorkShift(WorkShiftId.New(), employeeId, interval);
        workShift.AddDomainEvent(new WorkShiftCreated(workShift.Id, employeeId, interval));

        return workShift;
    }

    public void Reschedule(ShiftInterval newInterval)
    {
        if (_isConfirmed)
            throw new InvalidOperationException("Cannot reschedule a confirmed shift.");

        _interval = newInterval;
        AddDomainEvent(new WorkShiftRescheduled(Id, newInterval));
    }

    public void Confirm()
    {
        if (_isConfirmed)
            return;

        _isConfirmed = true;
        AddDomainEvent(new WorkShiftConfirmed(Id));
    }
}