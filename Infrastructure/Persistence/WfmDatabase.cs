namespace WfmApp.Presentation.Infrastructure.Persistence;

public static class WfmDatabase
{
    public static string ConnectionString { get; } =
        Environment.GetEnvironmentVariable("WFM_CONNECTION_STRING")
            ?? throw new InvalidOperationException(
                "Environment variable WFM_CONNECTION_STRING is not set. " +
                "Expected format: Host=<host>;Port=5432;Database=<db>;Username=<user>;Password=<password>");
}
