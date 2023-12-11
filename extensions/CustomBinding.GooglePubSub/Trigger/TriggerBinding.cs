using System.Reflection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomBinding.GooglePubSub;

internal sealed class TriggerBinding : ITriggerBinding
{
    private readonly ParameterInfo _parameter;
    private readonly IOptions<BindingOptions> _options;
    private readonly GPubSubTriggerAttribute _config;
    private readonly ILogger _logger;

    private readonly INameResolver _nameResolver;

    public Type TriggerValueType => typeof(GPubSubReceivedMessageModel);

    public IReadOnlyDictionary<string, Type> BindingDataContract => new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

    public TriggerBinding(ParameterInfo parameter,
        IOptions<BindingOptions> options,
        GPubSubTriggerAttribute config,
        ILogger logger,
        INameResolver nameResolver)
    {
        _parameter = parameter;
        _options = options;
        _config = config;
        _logger = logger;
        _nameResolver = nameResolver;
    }

    public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
    {
        var data = value as GPubSubReceivedMessageModel ?? new GPubSubReceivedMessageModel();

        var valueProvider = new ValueProvider(data);
        var bindingData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        return Task.FromResult<ITriggerData>(new TriggerData(valueProvider, bindingData));
    }

    public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return Task.FromResult<IListener>(new TriggerListener(context.Executor, _options, _config, _logger, _nameResolver));
    }

    public ParameterDescriptor ToParameterDescriptor() => new BindingTriggerParameterDescriptor
    {
        Name = _parameter.Name
    };

    private class ValueProvider : IValueProvider
    {
        private readonly object _value;

        public ValueProvider(object value) => _value = value;

        public Type Type => typeof(GPubSubReceivedMessageModel);

        public Task<object> GetValueAsync() => Task.FromResult(_value);

        public string ToInvokeString() => string.Empty;
    }

    private class BindingTriggerParameterDescriptor : TriggerParameterDescriptor
    {
        public override string GetTriggerReason(IDictionary<string, string> _) => "Trigger fired on schedule";
    }
}