using Microsoft.Azure.WebJobs.Description;

namespace CustomBinding.GooglePubSub;

[Binding]
[AttributeUsage(AttributeTargets.Parameter)]
public class GPubSubTriggerAttribute : GPubSubAttribute { }
