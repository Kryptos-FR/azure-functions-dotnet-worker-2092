using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Logging;

namespace CustomBinding.SFTP;

[Extension("SFTPBinding")]
public sealed class SFTPBindingExtension : IExtensionConfigProvider
{
    private readonly ILogger<SFTPBindingExtension> _logger;
    private readonly TelemetryClient _telemetryClient;

    public SFTPBindingExtension(
        ILogger<SFTPBindingExtension> logger,
        TelemetryConfiguration telemetryConfiguration)
    {
        _logger = logger;
        _telemetryClient = new TelemetryClient(telemetryConfiguration);
    }

    public void Initialize(ExtensionConfigContext context)
    {
        var rule = context.AddBindingRule<SFTPBindingAttribute>();
        rule.BindToInput(GetController);
    }

    private ISFTPController GetController(SFTPBindingAttribute arg)
    {
        return new SFTPController(arg, _logger, _telemetryClient);
    }
}
