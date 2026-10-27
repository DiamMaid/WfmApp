namespace WfmApp.Contracts;

public sealed record HubDto(
    Guid Id,
    string Code,
    string Name,
    string City,
    string Site,
    string TimeZone,
    bool IsActive);
