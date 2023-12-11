using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomBinding.GooglePubSub;

[Extension("GPubSubBinding")]
public sealed class GPubSubExtension : IExtensionConfigProvider
{
    private readonly IConverterManager _converterManager;
    private readonly ILogger<GPubSubExtension> _logger;
    private readonly IOptions<BindingOptions> _options;
    private readonly INameResolver _nameResolver;

    public GPubSubExtension(
        ILogger<GPubSubExtension> logger,
        IOptions<BindingOptions> options,
        INameResolver nameResolver,
        IConverterManager converterManager)
    {
        _logger = logger;
        _options = options;
        _nameResolver = nameResolver;
        _converterManager = converterManager;
    }

    public void Initialize(ExtensionConfigContext context)
    {
        ArgumentNullException.ThrowIfNull(nameof(context));

        context
            .AddConverter(new MessageToStringConverter()); // Ugly hack

        context
            .AddBindingRule<GPubSubTriggerAttribute>()
            .BindToTrigger(new TriggerBindingProvider(_logger, _options, _nameResolver, _converterManager));

        context
            .AddBindingRule<GPubSubBindingAttribute>()
            .AddConverter(new ControllerToStringConverter()) // Ugly hack
            .BindToInput(BuildController);
    }

    private IGPubSubController BuildController(GPubSubBindingAttribute attribute)
    {
        var config = new ControllerConfig
        (
            ProjectId: Resolve(attribute.ProjectId),
            JwtToken: Resolve(attribute.JwtToken),
            SubscriptionName: Resolve(attribute.SubscriptionName)
        );
        return new GPubSubController(config, _logger, _options?.Value);
    }

    private string Resolve(string name)
    {
        return _nameResolver.TryResolveWholeString(name, out var resolved) ? resolved : name;
    }
}

internal sealed class ControllerToStringConverter : IConverter<IGPubSubController, string>
{
    public string Convert(IGPubSubController input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return JsonSerializer.Serialize(((GPubSubController)input).Config);
    }
}

internal sealed class MessageToStringConverter : IConverter<GPubSubReceivedMessageModel, string>
{
    public string Convert(GPubSubReceivedMessageModel input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return JsonSerializer.Serialize(input);
    }
}
