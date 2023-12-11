namespace CustomBinding.GooglePubSub;

public interface IGPubSubController
{
    Task<string> Publish(string topic, GPubSubPublishedMessageModel message);
    IAsyncEnumerable<GPubSubReceivedMessageModel> GetMessages();
    Task AcknowledgeAsync(string ackId);
}
