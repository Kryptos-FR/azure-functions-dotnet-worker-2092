namespace Functions.Isolated;

internal struct EnvironmentVariables
{
    public const string APPCONFIG_CONNECTIONSTRING = "appConfigConnectionString";
    public const string APPCONFIG_LABEL = "appConfigLabel";
}

public struct GPubSubTopics
{
    public const string GOOGLE_SUBSCRIPTION = "AB_CDEF_TopicName_GCPQ";
}

public struct ServiceBusTopics
{
    public const string SB_TOPIC = "ttopicname";
}
