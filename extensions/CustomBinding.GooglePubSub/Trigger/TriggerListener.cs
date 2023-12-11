using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomBinding.GooglePubSub;

[Singleton(Mode = SingletonMode.Listener)]
public sealed class TriggerListener : IListener
{
    private readonly ITriggeredFunctionExecutor _executor;
    private readonly IOptions<BindingOptions> _options;
    private readonly GPubSubTriggerAttribute _config;
    private readonly ILogger _logger;
    private readonly INameResolver _nameResolver;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly IGPubSubController _controller;
    private readonly System.Timers.Timer _triggerTimer;

    private bool _disposed;

    public TriggerListener(ITriggeredFunctionExecutor executor,
        IOptions<BindingOptions> options,
        GPubSubTriggerAttribute config,
        ILogger logger,
        INameResolver nameResolver)
    {
        _executor = executor;
        _options = options;
        _logger = logger;

        _config = new GPubSubTriggerAttribute
        {
            ProjectId = nameResolver.TryResolveWholeString(config.ProjectId, out var id) ? id : config.ProjectId,
            JwtToken = nameResolver.TryResolveWholeString(config.JwtToken, out var jwt) ? jwt : config.JwtToken,
            SubscriptionName = config.SubscriptionName
        };
        _nameResolver = nameResolver;

        _controller = new GPubSubController(_config, logger, options);


        _triggerTimer = new System.Timers.Timer
        {
            Interval = (double)(_options.Value?.PollingInterval ?? 15) * 1000,
            AutoReset = false //true;
        };
        _triggerTimer.Elapsed += OnSchedule;

        _cancellationTokenSource = new CancellationTokenSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        _triggerTimer.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        _cancellationTokenSource.Cancel();
        return Task.FromResult(true);
    }

    public void Cancel()
    {
        ThrowIfDisposed();

        _cancellationTokenSource.Cancel();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            _disposed = true;
        }
    }

    private async void OnSchedule(Object source, System.Timers.ElapsedEventArgs e)
    {
        await PollAPI();

        _triggerTimer.Stop();
        _triggerTimer.Start();
    }

    /// <summary>
    /// Invokes the job function.
    /// </summary>
    internal async Task PollAPI()
    {
        var token = _cancellationTokenSource.Token;

        try
        {
            await foreach (var data in _controller.GetMessages())
            {
                var input = new TriggeredFunctionData
                {
                    TriggerValue = data
                };

                try
                {
                    var result = await _executor.TryExecuteAsync(input, token);
                    if (!result.Succeeded)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, exc.Message);
                    // We don't want any function errors to stop the execution
                    // schedule. Errors will be logged to Dashboard already.
                }
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, exc.Message);
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(TriggerListener));
        }
    }
}
