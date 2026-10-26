using WfmApp.Domain.Common;

namespace WfmApp.Application.UseCases.CheckOut;

public sealed record CheckOutResult(
    WarehouseShiftId ShiftId,
    int WorkedMinutes,
    int BreakMinutes,
    decimal? UnitsPerHour,
    string OutputBasis);
