namespace WfmApp.Domain.Common;

public readonly record struct WorkShiftId(Guid Value)
{
    public static WorkShiftId New() => new(Guid.NewGuid());
}