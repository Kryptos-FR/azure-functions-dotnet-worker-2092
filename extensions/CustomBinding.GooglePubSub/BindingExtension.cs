using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomBinding.GooglePubSub;

[Extension("GPubSubBinding")]
public sealed class GPubSubExtension : IExtensionConfigProvider
{
    private readonly ILogger<GPubSubExtension> _logger;
    private readonly IOptions<BindingOptions> _options;
    private readonly INameResolver _nameResolver;

    public GPubSubExtension(
        ILogger<GPubSubExtension> logger,
        IOptions<BindingOptions> options,
        INameResolver nameResolver)
    {
        _logger = logger;
        _options = options;
        _nameResolver = nameResolver;
    }

    public void Initialize(ExtensionConfigContext context)
    {
        ArgumentNullException.ThrowIfNull(nameof(context));

        var triggerRule = context.AddBindingRule<GPubSubTriggerAttribute>();
        triggerRule.BindToTrigger(new TriggerBindingProvider(_logger, _options, _nameResolver));

        var rule = context.AddBindingRule<GPubSubBindingAttribute>();
        rule.BindToInput(BuildController);
    }

    private IGPubSubController BuildController(GPubSubBindingAttribute config)
    {
        return new GPubSubController(config, _logger, _options);
    }
}