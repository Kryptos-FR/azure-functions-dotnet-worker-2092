using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;

namespace CustomBinding.GooglePubSub.Worker;

public sealed class GPubSubTriggerAttribute : TriggerBindingAttribute
{
    public string ProjectId { get; init; } = string.Empty;

    public string JwtToken { get; init; } = string.Empty;

    public string SubscriptionName { get; init; } = string.Empty;
}
