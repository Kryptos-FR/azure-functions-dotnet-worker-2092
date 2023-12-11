using System.Text.Json;
using Azure.Messaging.ServiceBus;
using CustomBinding.GooglePubSub;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Functions.InProcess;

public sealed class GPubSubSample
{
    [FunctionName(nameof(GPubSubSample))]
    public async Task Run(
        [GPubSubTrigger(
                ProjectId="%googlepubsub:projectid%",
                JwtToken="%googlepubsub:jwt%",
                SubscriptionName = GPubSubTopics.GOOGLE_SUBSCRIPTION)] GPubSubReceivedMessageModel receivedMessage,
        [GPubSubBinding(
                ProjectId="%googlepubsub:projectid%",
                JwtToken="%googlepubsub:jwt%",
                SubscriptionName = GPubSubTopics.GOOGLE_SUBSCRIPTION)] IGPubSubController gPubSubController,
        [ServiceBus(ServiceBusTopics.SB_TOPIC, Connection = "ServiceBus_ConnectionString")] ICollector<ServiceBusMessage> queueCollector,
        ILogger log
    )
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

        try
        {
            var data = JsonSerializer.SerializeToUtf8Bytes(receivedMessageData);
            ServiceBusMessage message = new ServiceBusMessage(data)
            {
                ApplicationProperties =
                {
                    {"MessageId", receivedMessage.messageId}
                }
            };
            queueCollector.Add(message);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Publishing message to service bus");
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
    }

    public sealed class ModelDTO
    {
        public string Value { get; set; }
    }
}
