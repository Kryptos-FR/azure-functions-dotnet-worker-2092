using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomBinding.GooglePubSub;

public partial class GPubSubController : IGPubSubController
{
    private readonly ILogger _logger;
    private readonly BindingOptions _options;
    private readonly GPubSubAttribute _config;

    public GPubSubController(GPubSubAttribute config, ILogger logger, IOptions<BindingOptions> options)
    {
        _logger = logger;
        _config = config;
        _options = options?.Value;

        _logger.LogDebug($"Controller : {config.ProjectId}");
    }

    public Task AcknowledgeAsync(string ackId)
    {
        _logger.LogDebug(nameof(AcknowledgeAsync));

        // NOTE: mock implementation
        return Task.CompletedTask;
    }

    public Task<string> Publish(string topic, GPubSubPublishedMessageModel message)
    {
        _logger.LogDebug(nameof(Publish));

        // NOTE: mock implementation
        return Task.FromResult(string.Empty);
    }

    public async IAsyncEnumerable<GPubSubReceivedMessageModel> GetMessages()
    {
        _logger.LogDebug(nameof(GetMessages));

        // NOTE: mock implementation
        await Task.Yield();
        yield break;
    }
}
