namespace CustomBinding.GooglePubSub;

public class GPubSubPublishedMessageModel
{
    public string data { get; set; }
    public Dictionary<string, string> attributes { get; set; }
}
