using System.Collections.ObjectModel;
using WfmApp.Contracts;
using WfmApp.Domain.Common;
using WfmApp.Domain.Enums;
using WfmApp.Presentation.Api;

namespace WfmApp.Presentation.ViewModels;

public sealed class ShiftDashboardViewModel : ViewModelBase
{
    private static readonly TimeSpan LateGrace = TimeSpan.FromMinutes(15);
    private const int BreakMinutes = 45;

    private readonly IWfmApiClient _client;
    private CancellationTokenSource? _loadCts;

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

    public ShiftDashboardViewModel(IWfmApiClient client)
    {
        _client = client;

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

    private CancellationToken BeginLoad()
    {
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = new CancellationTokenSource();

        return _loadCts.Token;
    }

    private async Task LoadHubsAsync()
    {
        var ct = BeginLoad();

        try
        {
            var hubs = await _client.GetHubsAsync(ct);
            Hubs.Clear();

            foreach (var hub in hubs)
                Hubs.Add(new HubOption(new HubId(hub.Id), $"{hub.Code} ({hub.Site})"));

            DbStatusText = "API: подключено";

            if (Hubs.Count > 0)
                SelectedHub = Hubs[0];
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            DbStatusText = "API: ошибка";
            ErrorMessage = ex.Message;
        }
    }

    private async Task LoadEmployeesAndShiftsAsync()
    {
        var ct = BeginLoad();

        await LoadEmployeesAsync(ct);
        await ReloadShiftsAsync(ct);
    }

    private async Task LoadEmployeesAsync(CancellationToken ct)
    {
        Employees.Clear();

        if (_selectedHub is null)
            return;

        try
        {
            var employees = await _client.GetEmployeesAsync(_selectedHub.Id.Value, ct);

            foreach (var employee in employees)
                Employees.Add(new EmployeeOption(new EmployeeId(employee.Id), $"{employee.StaffId} — {employee.FullName}"));

            DbStatusText = "API: подключено";
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            DbStatusText = "API: ошибка";
            ErrorMessage = ex.Message;
        }
    }

    private Task ReloadShiftsAsync() => ReloadShiftsAsync(BeginLoad());

    private async Task ReloadShiftsAsync(CancellationToken ct)
    {
        Shifts.Clear();

        if (_selectedHub is null)
            return;

        try
        {
            var day = DayStart();
            var now = DateTimeOffset.UtcNow;
            var shifts = await _client.GetShiftsAsync(_selectedHub.Id.Value, day, ct);
            var employees = await _client.GetEmployeesAsync(_selectedHub.Id.Value, ct);
            var byId = employees.ToDictionary(e => e.Id);

            foreach (var shift in shifts)
            {
                if (!byId.TryGetValue(shift.EmployeeId, out var employee))
                    continue;

                Shifts.Add(new ShiftRowViewModel(
                    new WarehouseShiftId(shift.ShiftId),
                    new EmployeeId(shift.EmployeeId),
                    new HubId(shift.HubId),
                    employee.StaffId,
                    employee.FullName,
                    RoleText(employee.Role),
                    ZoneText(shift.Zone),
                    shift.PlannedStart,
                    shift.PlannedEnd,
                    ToAttendance(shift, now),
                    shift.Status == ShiftStatus.Planned,
                    shift.Status == ShiftStatus.Active,
                    new AsyncRelayCommand(() => CheckInAsync(shift)),
                    new AsyncRelayCommand(() => CheckOutAsync(shift))));
            }

            TotalOnShift = Shifts.Count(r => r.Attendance == AttendanceStatus.OnShift || r.Attendance == AttendanceStatus.Late);
            ReceivingCount = Shifts.Count(r => r.Zone == ZoneText(WorkZone.Receiving) && r.Attendance != AttendanceStatus.Finished);
            PackingCount = Shifts.Count(r => r.Zone == ZoneText(WorkZone.Packing) && r.Attendance != AttendanceStatus.Finished);
            DbStatusText = "API: подключено";
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            DbStatusText = "API: ошибка";
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

        var ct = BeginLoad();
        var day = DayStart();
        var start = (_isNightPreset ? day.AddHours(20) : day.AddHours(8)).ToUniversalTime();
        var end = (_isNightPreset ? day.AddDays(1).AddHours(8) : day.AddHours(20)).ToUniversalTime();

        try
        {
            var dayShifts = await _client.GetShiftsAsync(_selectedHub.Id.Value, start, ct);

            if (dayShifts.Any(s => s.EmployeeId == _selectedEmployee.Id.Value))
            {
                ErrorMessage = "Сотрудник уже назначен на смену в эту дату.";
                return;
            }

            var request = new CreateShiftRequest
            {
                EmployeeId = _selectedEmployee.Id.Value,
                Zone = _selectedZone,
                Start = start,
                End = end,
                BreakStart = start.AddHours(5),
                BreakMinutes = BreakMinutes
            };

            var created = await _client.CreateShiftAsync(request, ct);
            StatusMessage = $"Смена назначена: {created.ShiftId}";
            await ReloadShiftsAsync(ct);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ErrorMessage = ex.Message;
        }
    }

    private async Task CheckInAsync(ShiftDto shift)
    {
        ErrorMessage = null;
        var ct = BeginLoad();

        try
        {
            var request = new CheckInRequest
            {
                EmployeeId = shift.EmployeeId,
                HubId = shift.HubId,
                OccurredAt = DateTimeOffset.UtcNow,
                AccessControlEventId = $"MANUAL-{Guid.NewGuid():N}"
            };

            await _client.CheckInAsync(request, ct);
            await ReloadShiftsAsync(ct);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ErrorMessage = ex.Message;
        }
    }

    private async Task CheckOutAsync(ShiftDto shift)
    {
        ErrorMessage = null;
        var ct = BeginLoad();

        try
        {
            var request = new CheckOutRequest
            {
                ShiftId = shift.ShiftId,
                OccurredAt = DateTimeOffset.UtcNow
            };

            await _client.CheckOutAsync(request, ct);
            await ReloadShiftsAsync(ct);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            ErrorMessage = ex.Message;
        }
    }

    private DateTimeOffset DayStart() => new DateTimeOffset(_selectedDate);

    private static AttendanceStatus ToAttendance(ShiftDto shift, DateTimeOffset now)
    {
        if (shift.Status == ShiftStatus.Closed)
            return AttendanceStatus.Finished;

        if (shift.Status == ShiftStatus.Active)
            return shift.CheckedInAt is { } checkedIn && checkedIn > shift.PlannedStart + LateGrace
                ? AttendanceStatus.Late
                : AttendanceStatus.OnShift;

        return now > shift.PlannedStart + LateGrace ? AttendanceStatus.Late : AttendanceStatus.Planned;
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
