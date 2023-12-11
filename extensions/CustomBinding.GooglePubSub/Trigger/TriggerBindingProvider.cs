using System.Reflection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomBinding.GooglePubSub;

public class TriggerBindingProvider : ITriggerBindingProvider
{
    private readonly ILogger<GPubSubExtension> _logger;
    private readonly IOptions<BindingOptions> _options;

    private readonly INameResolver _nameResolver;
    private GPubSubTriggerAttribute _config;

    public TriggerBindingProvider(ILogger<GPubSubExtension> logger,
        IOptions<BindingOptions> options,
        INameResolver nameResolver)
    {
        _logger = logger;
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _nameResolver = nameResolver;
    }


    public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var parameter = context.Parameter;
        var attribute = parameter.GetCustomAttribute<GPubSubTriggerAttribute>(inherit: true);

        if (attribute is null)
        {
            return Task.FromResult<ITriggerBinding>(null);
        }
        _config = attribute;

        if (parameter.ParameterType != typeof(GPubSubReceivedMessageModel))
        {
            throw new InvalidOperationException(
                string.Format("Can't bind attribute to type '{0}'.", parameter.ParameterType));
        }

        return Task.FromResult<ITriggerBinding>(new TriggerBinding(parameter, _options, _config, _logger, _nameResolver));
    }
}
