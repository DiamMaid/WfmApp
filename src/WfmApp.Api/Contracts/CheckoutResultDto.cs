namespace WfmApp.Api.Contracts;

public sealed record CheckoutResultDto(
    Guid ShiftId,
    int WorkedMinutes,
    int BreakMinutes,
    decimal? UnitsPerHour,
    string OutputBasis);
