using System.Text.Json;
using CustomBinding.GooglePubSub;
using CustomBinding.GooglePubSub.Worker;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Functions.Isolated;

public sealed class GPubSubSample
{
    [Function(nameof(GPubSubSample))]
    [ServiceBusOutput(ServiceBusTopics.SB_TOPIC, Connection = "ServiceBus_ConnectionString")]
    public async Task<byte[]> Run(
        [GPubSubTrigger(
                ProjectId="%googlepubsub:projectid%", // issue: those aren't resolved if coming from Azure AppConfig
                JwtToken="%googlepubsub:jwt%",        // though it works from appSettings
                SubscriptionName = GPubSubTopics.GOOGLE_SUBSCRIPTION)] GPubSubReceivedMessageModel receivedMessage,
        [GPubSubBinding(
                ProjectId="%googlepubsub:projectid%",
                JwtToken="%googlepubsub:jwt%",
                SubscriptionName = GPubSubTopics.GOOGLE_SUBSCRIPTION)] IGPubSubController gPubSubController,
        ILogger log)
    {
        ModelDTO receivedMessageData;
        try
        {
            receivedMessageData = JsonSerializer.Deserialize<ModelDTO>(receivedMessage.data);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Deserializing message");
            throw;
        }

        byte[] data;
        try
        {
            data = JsonSerializer.SerializeToUtf8Bytes(receivedMessageData);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Preparing message to publish to service bus");
            throw;
        }

        try
        {
            await gPubSubController.AcknowledgeAsync(receivedMessage.ackId);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Acknowledge");
            throw;
        }

        // Not ideal: we would like to ensure the message has been published to ServiceBus before acknowledging it
        // There should be a way to have client to send the message however it is not documented in the following pages (bad discovery):
        //  - https://learn.microsoft.com/en-us/azure/azure-functions/migrate-dotnet-to-isolated-model
        //  - https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-service-bus
        //  - https://learn.microsoft.com/en-us/dotnet/api/azure.messaging.servicebus
        return data;
    }

    public sealed class ModelDTO
    {
        public string Value { get; set; }
    }
}
