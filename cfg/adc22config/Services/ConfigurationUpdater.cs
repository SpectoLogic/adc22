using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System.Text.Json;

namespace adc22config.Services;
public class ConfigurationUpdater : IConfigurationUpdater
{
    private readonly ILogger<ConfigurationUpdater> _logger;
    private Task _workerTask;
    private readonly CancellationTokenSource cts = new CancellationTokenSource();
    private readonly IConfigurationRefresher _refresher;

    public ConfigurationUpdater(ILogger<ConfigurationUpdater> logger,
            IConfigurationRefresher refresher)
    {
        _logger = logger;
        _refresher = refresher;
    }

    public async Task StartMonitoring(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Start monitoring,... ");
        _workerTask = Task.Run(async () => await Monitor(cts.Token));
    }
    public async Task StopMonitoring(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Stop monitoring,... ");
        cts.Cancel();
        return;
    }
    private async Task Monitor(CancellationToken cancellationToken)
    {
        var client = new ServiceBusClient("adc22config.servicebus.windows.net", new DefaultAzureCredential());
        var receiver = client.CreateReceiver("cfgtopic", "cfgSubscription");

        while (!cancellationToken.IsCancellationRequested)
        {
            var receivedMessage = await receiver.ReceiveMessageAsync();
            if (receivedMessage != null)
            {
                try
                {
                    var messageText = receivedMessage.Body.ToString();
                    var rootData = JsonDocument.Parse(messageText).RootElement;
                    var messageData = rootData.GetProperty("data");
                    var key = messageData.GetProperty("key").GetString();
                    var syncToken = messageData.GetProperty("syncToken").GetString();
                    var eventType = rootData.GetProperty("eventType").GetString();
                    var subject = rootData.GetProperty("subject").GetString() ?? "https://nowhere.com";
                    _logger.LogInformation($"Event received for Key = {key}");
                    _refresher.ProcessPushNotification(
                        new PushNotification()
                        {
                            SyncToken = syncToken,
                            EventType = eventType,
                            ResourceUri = new Uri(subject)
                        }, new TimeSpan(0, 0, 0));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process message!");
                }
            }
            await Task.Delay(1000, cancellationToken);
        }
    }
}
