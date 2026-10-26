namespace WfmApp.Domain.Common;

public readonly record struct WarehouseShiftId(Guid Value)
{
    public static WarehouseShiftId New() => new(Guid.NewGuid());
}
