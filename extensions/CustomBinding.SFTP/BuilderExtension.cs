using Microsoft.Azure.WebJobs;

namespace CustomBinding.SFTP;

public static class BuilderExtension
{
    public static IWebJobsBuilder AddSFTPBinding(this IWebJobsBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(nameof(builder));

        builder.AddExtension<SFTPBindingExtension>();
        return builder;
    }
}
