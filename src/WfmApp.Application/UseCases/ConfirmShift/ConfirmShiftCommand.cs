using WfmApp.Domain.Common;

namespace WfmApp.Application.UseCases.ConfirmShift;

public sealed class ConfirmShiftCommand
{
    public required WarehouseShiftId ShiftId { get; init; }
}
