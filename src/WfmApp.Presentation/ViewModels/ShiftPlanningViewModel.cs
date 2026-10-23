using WfmApp.Application.UseCases.CreateShift;
using WfmApp.Domain.Common;
using WfmApp.Domain.ValueObjects;

namespace WfmApp.Presentation.ViewModels;

public sealed class ShiftPlanningViewModel : ViewModelBase
{
    private readonly CreateShiftUseCase _useCase;
    private string _employeeIdInput = string.Empty;
    private DateTimeOffset _start;
    private DateTimeOffset _end;
    private string? _errorMessage;
    private string? _statusMessage;

    public ShiftPlanningViewModel(CreateShiftUseCase useCase)
    {
        _useCase = useCase;
        _start = RoundUpToNextHour(DateTimeOffset.Now);
        _end = _start.AddHours(8);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
    }

    public string EmployeeIdInput
    {
        get => _employeeIdInput;
        set => SetProperty(ref _employeeIdInput, value);
    }

    public DateTimeOffset Start
    {
        get => _start;
        set => SetProperty(ref _start, value);
    }

    public DateTimeOffset End
    {
        get => _end;
        set => SetProperty(ref _end, value);
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

    public AsyncRelayCommand SaveCommand { get; }

    private async Task SaveAsync()
    {
        ErrorMessage = null;
        StatusMessage = null;

        if (!Guid.TryParse(EmployeeIdInput, out var employeeGuid))
        {
            ErrorMessage = "EmployeeId должен быть валидным GUID.";
            return;
        }

        ShiftInterval interval;
        try
        {
            interval = ShiftInterval.Create(Start, End);
        }
        catch (ArgumentException ex)
        {
            ErrorMessage = ex.Message;
            return;
        }

        var command = new CreateShiftCommand
        {
            EmployeeId = new EmployeeId(employeeGuid),
            Start = interval.Start,
            End = interval.End
        };

        var shiftId = await _useCase.ExecuteAsync(command);
        StatusMessage = $"Смена создана: {shiftId.Value}";
    }

    private static DateTimeOffset RoundUpToNextHour(DateTimeOffset now)
    {
        var hour = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 0, now.Offset);
        return hour.AddHours(1);
    }
}