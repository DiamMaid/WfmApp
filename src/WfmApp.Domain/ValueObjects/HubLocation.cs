namespace WfmApp.Domain.ValueObjects;

public sealed record HubLocation
{
    private HubLocation(string city, string site)
    {
        City = city;
        Site = site;
    }

    public string City { get; }
    public string Site { get; }

    public static HubLocation Create(string city, string site)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City must not be empty.", nameof(city));

        if (string.IsNullOrWhiteSpace(site))
            throw new ArgumentException("Site must not be empty.", nameof(site));

        return new HubLocation(city, site);
    }
}
