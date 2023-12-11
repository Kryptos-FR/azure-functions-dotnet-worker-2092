using Microsoft.Azure.WebJobs;

namespace CustomBinding.GooglePubSub;

public static class BuilderExtension
{
    public static IWebJobsBuilder AddGPubSubBinding(this IWebJobsBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(nameof(builder));

        builder.AddExtension<GPubSubExtension>();
        return builder;
    }
}