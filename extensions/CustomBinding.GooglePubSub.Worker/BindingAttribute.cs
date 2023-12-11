using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;

namespace CustomBinding.GooglePubSub.Worker;

[AttributeUsage(AttributeTargets.Parameter)]
public class GPubSubBindingAttribute : InputBindingAttribute
{
    public string ProjectId { get; init; } = string.Empty;

    public string JwtToken { get; init; } = string.Empty;

    public string SubscriptionName { get; init; } = string.Empty;
}
