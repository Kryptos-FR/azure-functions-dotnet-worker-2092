using System.Text.Json;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
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
        ArgumentNullException.ThrowIfNull(nameof(context));

        context
            .AddBindingRule<SFTPBindingAttribute>()
            .AddConverter(new ControllerToStringConverter())
            .BindToInput(GetController);
    }

    private ISFTPController GetController(SFTPBindingAttribute attribute)
    {
        var config = new ControllerConfig
        (
            Host: attribute.Host,
            Port: attribute.Port,
            Login: attribute.Login,
            Password: attribute.Password,
            RsaKey: attribute.RsaKey
        );
        return new SFTPController(config, _logger, _telemetryClient);
    }
}

internal sealed class ControllerToStringConverter : IConverter<ISFTPController, string>
{
    public string Convert(ISFTPController input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return JsonSerializer.Serialize(((SFTPController)input).Config);
    }

    private record GPubSubInputData(string projectId, string jwtToken, string subscriptionName);
}
