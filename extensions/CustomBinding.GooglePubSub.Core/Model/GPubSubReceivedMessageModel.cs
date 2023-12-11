namespace CustomBinding.GooglePubSub;

public class GPubSubReceivedMessageModel : GPubSubPublishedMessageModel
{
    public string messageId { get; set; }
    public string ackId { get; set; }
    public int deliveryAttempt { get; set; }
    public string orderingKey { get; set; }
    public DateTime publishingTime { get; set; }
}
