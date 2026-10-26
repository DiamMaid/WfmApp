namespace WfmApp.Domain.Common;

public readonly record struct EmployeeId(Guid Value)
{
    public static EmployeeId New() => new(Guid.NewGuid());
}
