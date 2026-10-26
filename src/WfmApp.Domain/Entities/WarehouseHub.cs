using System.Text.RegularExpressions;
using WfmApp.Domain.Common;
using WfmApp.Domain.Events;
using WfmApp.Domain.ValueObjects;

namespace WfmApp.Domain.Entities;

public sealed partial class WarehouseHub : AggregateRoot<HubId>
{
    private string _code;
    private string _name;
    private HubLocation _location;
    private string _timeZone;
    private bool _isActive;

    private WarehouseHub()
    {
        _code = null!;
        _name = null!;
        _location = null!;
        _timeZone = null!;
    }

    private WarehouseHub(HubId id, string code, string name, HubLocation location, string timeZone)
        : base(id)
    {
        _code = code;
        _name = name;
        _location = location;
        _timeZone = timeZone;
        _isActive = true;
    }

    public string Code => _code;
    public string Name => _name;
    public HubLocation Location => _location;
    public string TimeZone => _timeZone;
    public bool IsActive => _isActive;

    public static WarehouseHub Create(string code, string name, HubLocation location, string timeZone)
    {
        ValidateCode(code);
        ValidateName(name);
        ValidateTimeZone(timeZone);

        var hub = new WarehouseHub(HubId.New(), code, name, location, timeZone);
        hub.AddDomainEvent(new WarehouseHubCreated(hub.Id, code, name, location, timeZone));

        return hub;
    }

    public void Activate()
    {
        if (_isActive)
            throw new InvalidOperationException("Hub is already active.");

        _isActive = true;
        AddDomainEvent(new WarehouseHubActivated(Id));
    }

    public void Deactivate()
    {
        if (!_isActive)
            throw new InvalidOperationException("Hub is already inactive.");

        _isActive = false;
        AddDomainEvent(new WarehouseHubDeactivated(Id));
    }

    public void Rename(string name)
    {
        ValidateName(name);

        _name = name;
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code must not be empty.", nameof(code));

        if (!CodePattern().IsMatch(code))
            throw new ArgumentException("Code must be 3 to 32 characters of uppercase letters, digits or hyphens.", nameof(code));
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name must not be empty.", nameof(name));
    }

    private static void ValidateTimeZone(string timeZone)
    {
        if (string.IsNullOrWhiteSpace(timeZone))
            throw new ArgumentException("Time zone must not be empty.", nameof(timeZone));

        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            throw new ArgumentException($"Unknown IANA time zone '{timeZone}'.", nameof(timeZone));
        }
        catch (InvalidTimeZoneException)
        {
            throw new ArgumentException($"Time zone '{timeZone}' has invalid data.", nameof(timeZone));
        }
    }

    [GeneratedRegex("^[A-Z0-9-]{3,32}$")]
    private static partial Regex CodePattern();
}
