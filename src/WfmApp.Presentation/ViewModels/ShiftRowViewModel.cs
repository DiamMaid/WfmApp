using WfmApp.Domain.Common;

namespace WfmApp.Presentation.ViewModels;

public sealed class ShiftRowViewModel
{
    public ShiftRowViewModel(
        WarehouseShiftId shiftId,
        EmployeeId employeeId,
        HubId hubId,
        string staffId,
        string fullName,
        string role,
        string zone,
        DateTimeOffset start,
        DateTimeOffset end,
        AttendanceStatus attendance,
        bool canCheckIn,
        bool canCheckOut,
        AsyncRelayCommand checkInCommand,
        AsyncRelayCommand checkOutCommand)
    {
        ShiftId = shiftId;
        EmployeeId = employeeId;
        HubId = hubId;
        StaffId = staffId;
        FullName = fullName;
        Role = role;
        Zone = zone;
        Start = start;
        End = end;
        Attendance = attendance;
        CanCheckIn = canCheckIn;
        CanCheckOut = canCheckOut;
        CheckInCommand = checkInCommand;
        CheckOutCommand = checkOutCommand;
    }

    public WarehouseShiftId ShiftId { get; }
    public EmployeeId EmployeeId { get; }
    public HubId HubId { get; }
    public string StaffId { get; }
    public string FullName { get; }
    public string Role { get; }
    public string Zone { get; }
    public DateTimeOffset Start { get; }
    public DateTimeOffset End { get; }
    public AttendanceStatus Attendance { get; }

    public string AttendanceText => Attendance switch
    {
        AttendanceStatus.OnShift => "На смене",
        AttendanceStatus.Late => "Опоздание",
        AttendanceStatus.Finished => "Завершил",
        _ => "Запланирована"
    };

    public bool CanCheckIn { get; }
    public bool CanCheckOut { get; }
    public AsyncRelayCommand CheckInCommand { get; }
    public AsyncRelayCommand CheckOutCommand { get; }
}
