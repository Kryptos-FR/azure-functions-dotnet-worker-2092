using Microsoft.Azure.WebJobs.Description;

namespace CustomBinding.GooglePubSub;

[Binding]
[AttributeUsage(AttributeTargets.Parameter)]
public class GPubSubBindingAttribute : GPubSubAttribute { }

public class GPubSubAttribute : Attribute
{
    [AutoResolve]
    public string ProjectId { get; set; }

    [AutoResolve]
    public string JwtToken { get; set; }

    [AutoResolve]
    public string SubscriptionName { get; set; }
}
