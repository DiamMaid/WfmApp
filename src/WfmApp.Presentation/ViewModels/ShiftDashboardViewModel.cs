using System.Collections.ObjectModel;
using WfmApp.Application.Interfaces;
using WfmApp.Application.UseCases.CheckIn;
using WfmApp.Application.UseCases.CheckOut;
using WfmApp.Application.UseCases.CreateShift;
using WfmApp.Domain.Common;
using WfmApp.Domain.Entities;
using WfmApp.Domain.Enums;

namespace WfmApp.Presentation.ViewModels;

public sealed class ShiftDashboardViewModel : ViewModelBase
{
    private static readonly TimeSpan LateGrace = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan BreakDuration = TimeSpan.FromMinutes(45);

    private readonly IHubRepository _hubs;
    private readonly IEmployeeRepository _employees;
    private readonly IShiftRepository _shifts;
    private readonly CreateShiftUseCase _createShift;
    private readonly CheckInUseCase _checkIn;
    private readonly CheckOutUseCase _checkOut;

    private HubOption? _selectedHub;
    private WorkZone _selectedZone = WorkZone.Receiving;
    private DateTime _selectedDate = DateTime.Today;
    private EmployeeOption? _selectedEmployee;
    private bool _isNightPreset;
    private string? _errorMessage;
    private string? _statusMessage;
    private string _dbStatusText = "нет связи";
    private int _totalOnShift;
    private int _receivingCount;
    private int _packingCount;

    public ShiftDashboardViewModel(
        IHubRepository hubs,
        IEmployeeRepository employees,
        IShiftRepository shifts,
        CreateShiftUseCase createShift,
        CheckInUseCase checkIn,
        CheckOutUseCase checkOut)
    {
        _hubs = hubs;
        _employees = employees;
        _shifts = shifts;
        _createShift = createShift;
        _checkIn = checkIn;
        _checkOut = checkOut;

        Zones = Enum.GetValues<WorkZone>();
        ReloadCommand = new AsyncRelayCommand(ReloadShiftsAsync);
        AssignCommand = new AsyncRelayCommand(AssignAsync);

        _ = LoadHubsAsync();
    }

    public ObservableCollection<HubOption> Hubs { get; } = new();
    public ObservableCollection<EmployeeOption> Employees { get; } = new();
    public ObservableCollection<ShiftRowViewModel> Shifts { get; } = new();
    public WorkZone[] Zones { get; }
    public AsyncRelayCommand ReloadCommand { get; }
    public AsyncRelayCommand AssignCommand { get; }

    public HubOption? SelectedHub
    {
        get => _selectedHub;
        set
        {
            if (SetProperty(ref _selectedHub, value))
                _ = LoadEmployeesAndShiftsAsync();
        }
    }

    public WorkZone SelectedZone
    {
        get => _selectedZone;
        set => SetProperty(ref _selectedZone, value);
    }

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (SetProperty(ref _selectedDate, value))
                _ = ReloadShiftsAsync();
        }
    }

    public EmployeeOption? SelectedEmployee
    {
        get => _selectedEmployee;
        set => SetProperty(ref _selectedEmployee, value);
    }

    public bool IsNightPreset
    {
        get => _isNightPreset;
        set => SetProperty(ref _isNightPreset, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public string? StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public string DbStatusText
    {
        get => _dbStatusText;
        private set => SetProperty(ref _dbStatusText, value);
    }

    public int TotalOnShift
    {
        get => _totalOnShift;
        private set => SetProperty(ref _totalOnShift, value);
    }

    public int ReceivingCount
    {
        get => _receivingCount;
        private set => SetProperty(ref _receivingCount, value);
    }

    public int PackingCount
    {
        get => _packingCount;
        private set => SetProperty(ref _packingCount, value);
    }

    private async Task LoadHubsAsync()
    {
        try
        {
            var hubs = await _hubs.GetActiveAsync();
            Hubs.Clear();

            foreach (var hub in hubs)
                Hubs.Add(new HubOption(hub.Id, $"{hub.Code} ({hub.Location.Site})"));

            DbStatusText = "PostgreSQL: подключено";

            if (Hubs.Count > 0)
                SelectedHub = Hubs[0];
        }
        catch (Exception ex)
        {
            DbStatusText = "PostgreSQL: ошибка";
            ErrorMessage = ex.Message;
        }
    }

    private async Task LoadEmployeesAndShiftsAsync()
    {
        await LoadEmployeesAsync();
        await ReloadShiftsAsync();
    }

    private async Task LoadEmployeesAsync()
    {
        Employees.Clear();

        if (_selectedHub is null)
            return;

        try
        {
            var employees = await _employees.GetByHubAsync(_selectedHub.Id);

            foreach (var employee in employees)
                Employees.Add(new EmployeeOption(employee.Id, $"{employee.StaffId} — {employee.FullName}"));

            DbStatusText = "PostgreSQL: подключено";
        }
        catch (Exception ex)
        {
            DbStatusText = "PostgreSQL: ошибка";
            ErrorMessage = ex.Message;
        }
    }

    private async Task ReloadShiftsAsync()
    {
        Shifts.Clear();

        if (_selectedHub is null)
            return;

        try
        {
            var day = DayStart();
            var now = DateTimeOffset.Now;
            var shifts = await _shifts.GetByHubAndDateAsync(_selectedHub.Id, day, CancellationToken.None);
            var employees = await _employees.GetByHubAsync(_selectedHub.Id);
            var byId = employees.ToDictionary(e => e.Id);

            foreach (var shift in shifts)
            {
                if (!byId.TryGetValue(shift.EmployeeId, out var employee))
                    continue;

                var attendance = ToAttendance(shift, now);

                Shifts.Add(new ShiftRowViewModel(
                    shift.Id,
                    employee.Id,
                    shift.HubId,
                    employee.StaffId,
                    employee.FullName,
                    RoleText(employee.Role),
                    ZoneText(shift.Zone),
                    shift.Planned.Start,
                    shift.Planned.End,
                    attendance,
                    shift.Status == ShiftStatus.Planned,
                    shift.Status == ShiftStatus.Active,
                    new AsyncRelayCommand(() => CheckInAsync(shift)),
                    new AsyncRelayCommand(() => CheckOutAsync(shift))));
            }

            TotalOnShift = Shifts.Count(r => r.Attendance == AttendanceStatus.OnShift || r.Attendance == AttendanceStatus.Late);
            ReceivingCount = Shifts.Count(r => r.Zone == ZoneText(WorkZone.Receiving) && r.Attendance != AttendanceStatus.Finished);
            PackingCount = Shifts.Count(r => r.Zone == ZoneText(WorkZone.Packing) && r.Attendance != AttendanceStatus.Finished);
            DbStatusText = "PostgreSQL: подключено";
        }
        catch (Exception ex)
        {
            DbStatusText = "PostgreSQL: ошибка";
            ErrorMessage = ex.Message;
        }
    }

    private async Task AssignAsync()
    {
        ErrorMessage = null;
        StatusMessage = null;

        if (_selectedHub is null)
        {
            ErrorMessage = "Выберите логистический хаб.";
            return;
        }

        if (_selectedEmployee is null)
        {
            ErrorMessage = "Выберите сотрудника из списка.";
            return;
        }

        var day = DayStart();
        var start = _isNightPreset ? day.AddHours(20) : day.AddHours(8);
        var end = _isNightPreset ? day.AddDays(1).AddHours(8) : day.AddHours(20);

        try
        {
            var existing = await _shifts.GetByEmployeeAndDateAsync(_selectedEmployee.Id, start);

            if (existing.Count > 0)
            {
                ErrorMessage = "Сотрудник уже назначен на смену в эту дату.";
                return;
            }

            var command = new CreateShiftCommand
            {
                EmployeeId = _selectedEmployee.Id,
                Zone = _selectedZone,
                Start = start,
                End = end,
                BreakStart = start.AddHours(5),
                BreakDuration = BreakDuration
            };

            var shiftId = await _createShift.ExecuteAsync(command);
            StatusMessage = $"Смена назначена: {shiftId.Value}";
            await ReloadShiftsAsync();
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ErrorMessage = ex.Message;
        }
    }

    private async Task CheckInAsync(WarehouseShift shift)
    {
        ErrorMessage = null;

        try
        {
            var command = new CheckInCommand
            {
                EmployeeId = shift.EmployeeId,
                HubId = shift.HubId,
                OccurredAt = DateTimeOffset.Now,
                AccessControlEventId = $"MANUAL-{Guid.NewGuid():N}"
            };

            await _checkIn.ExecuteAsync(command);
            await ReloadShiftsAsync();
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or Application.Common.EntityNotFoundException)
        {
            ErrorMessage = ex.Message;
        }
    }

    private async Task CheckOutAsync(WarehouseShift shift)
    {
        ErrorMessage = null;

        try
        {
            var command = new CheckOutCommand
            {
                ShiftId = shift.Id,
                OccurredAt = DateTimeOffset.Now
            };

            await _checkOut.ExecuteAsync(command);
            await ReloadShiftsAsync();
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or Application.Common.EntityNotFoundException)
        {
            ErrorMessage = ex.Message;
        }
    }

    private DateTimeOffset DayStart() => new DateTimeOffset(_selectedDate);

    private static AttendanceStatus ToAttendance(WarehouseShift shift, DateTimeOffset now)
    {
        if (shift.Status == ShiftStatus.Closed)
            return AttendanceStatus.Finished;

        if (shift.Status == ShiftStatus.Active)
            return shift.CheckedInAt is { } checkedIn && checkedIn > shift.Planned.Start + LateGrace
                ? AttendanceStatus.Late
                : AttendanceStatus.OnShift;

        return now > shift.Planned.Start + LateGrace ? AttendanceStatus.Late : AttendanceStatus.Planned;
    }

    private static string RoleText(Role role) => role switch
    {
        Role.Inbound => "Приёмка",
        Role.Putaway => "Размещение",
        Role.Picking => "Комплектация",
        Role.Packing => "Упаковка",
        Role.Outbound => "Отгрузка",
        _ => role.ToString()
    };

    private static string ZoneText(WorkZone zone) => zone switch
    {
        WorkZone.Receiving => "Приёмка",
        WorkZone.Mezzanine => "Мезонин",
        WorkZone.Packing => "Упаковка",
        WorkZone.SortingLine => "Сортировка",
        _ => zone.ToString()
    };
}
