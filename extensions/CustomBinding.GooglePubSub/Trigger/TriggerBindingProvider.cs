using System.Reflection;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomBinding.GooglePubSub;

public class TriggerBindingProvider : ITriggerBindingProvider
{
    private readonly IConverterManager _converterManager;
    private readonly ILogger<GPubSubExtension> _logger;
    private readonly IOptions<BindingOptions> _options;

    private readonly INameResolver _nameResolver;

    public TriggerBindingProvider(ILogger<GPubSubExtension> logger,
        IOptions<BindingOptions> options,
        INameResolver nameResolver,
        IConverterManager converterManager)
    {
        _logger = logger;
        _options = options;
        _nameResolver = nameResolver;
        _converterManager = converterManager;
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

        var config = new ControllerConfig
        (
            ProjectId: Resolve(attribute.ProjectId),
            JwtToken: Resolve(attribute.JwtToken),
            SubscriptionName: Resolve(attribute.SubscriptionName)
        );

        if (parameter.ParameterType != typeof(GPubSubReceivedMessageModel))
        {
            throw new InvalidOperationException(
                string.Format("Can't bind attribute to type '{0}'.", parameter.ParameterType));
        }

        // BindingFactory is flagged as obsolte, hwoever it is the way it is done for ServiceBus, and the lack on any other documentation means we don't know how to do it differently
        var binding = BindingFactory.GetTriggerBinding(new GooglePubSubTriggerBindingStrategy(_logger), parameter, _converterManager, CreateListener);

        return Task.FromResult(binding);

        Task<IListener> CreateListener(ListenerFactoryContext context, bool singleDispatch)
        {
            IListener listener = new TriggerListener(context.Executor, _options, config, _logger);
            return Task.FromResult(listener);
        }
    }

    private string Resolve(string name)
    {
        return _nameResolver.TryResolveWholeString(name, out var resolved) ? resolved : name;
    }
}

// Very ugly hack: it is only needed to have a way to convert to a string (see ConvertFromString method)
internal sealed class GooglePubSubTriggerBindingStrategy : ITriggerBindingStrategy<GPubSubReceivedMessageModel, GPubSubReceivedMessageModel>
{
    private readonly ILogger _logger;

    public GooglePubSubTriggerBindingStrategy(ILogger logger)
    {
        _logger = logger;
    }
    
    public GPubSubReceivedMessageModel[] BindMultiple(GPubSubReceivedMessageModel value, ValueBindingContext context)
    {
        return [value];
    }

    public GPubSubReceivedMessageModel BindSingle(GPubSubReceivedMessageModel value, ValueBindingContext context)
    {
        return value;
    }

    public GPubSubReceivedMessageModel ConvertFromString(string message)
    {
        return JsonSerializer.Deserialize<GPubSubReceivedMessageModel>(message);
    }

    public Dictionary<string, Type> GetBindingContract(bool isSingleDispatch)
    {
        return new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
    }

    public Dictionary<string, object> GetBindingData(GPubSubReceivedMessageModel value)
    {
        return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }
}
