namespace CustomBinding.GooglePubSub;

public class BindingOptions
{
    public int PollingInterval { get; set; } = 5;
    public int MaxMessageRetrieved { get; set; } = 25;
    public bool LogBodies { get; set; } = false;
}