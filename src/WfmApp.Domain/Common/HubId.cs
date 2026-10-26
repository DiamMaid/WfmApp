namespace WfmApp.Domain.Common;

public readonly record struct HubId(Guid Value)
{
    public static HubId New() => new(Guid.NewGuid());
}
