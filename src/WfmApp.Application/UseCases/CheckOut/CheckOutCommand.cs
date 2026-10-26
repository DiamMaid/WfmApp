using WfmApp.Domain.Common;

namespace WfmApp.Application.UseCases.CheckOut;

public sealed class CheckOutCommand
{
    public required WarehouseShiftId ShiftId { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public int? UnitsProduced { get; init; }
}
