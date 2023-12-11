namespace Functions.Isolated;

internal static class Settings
{
    internal static readonly string AppConfigConnectionString = Environment.GetEnvironmentVariable(EnvironmentVariables.APPCONFIG_CONNECTIONSTRING)!;
    internal static readonly string AppConfigLabel = Environment.GetEnvironmentVariable(EnvironmentVariables.APPCONFIG_LABEL)!;
}
