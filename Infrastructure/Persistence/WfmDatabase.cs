namespace WfmApp.Presentation.Infrastructure.Persistence;

public static class WfmDatabase
{
    public static string ConnectionString { get; } =
        Environment.GetEnvironmentVariable("WFM_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=wfm;Username=postgres;Password=postgres";
}
