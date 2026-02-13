using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RequestsService.Application.Common.Interfaces;
using System.Text;
using System.Text.Json;

namespace RequestsService.Infrastructure.Messaging;

public class AzureQueueStoragePublisher : IRequestCreatedPublisher
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<AzureQueueStoragePublisher> _logger;

    public AzureQueueStoragePublisher(IConfiguration config, ILogger<AzureQueueStoragePublisher> logger)
    {
        _logger = logger;
        var connectionString = config["AzureQueueStorage:ConnectionString"] ?? throw new ArgumentNullException("AzureQueueStorage:ConnectionString");
        var queueName = config["AzureQueueStorage:QueueName"] ?? "request-created-queue";
        var versionString = config["AzureQueueStorage:ServiceVersion"];

        QueueClientOptions options;
        if (!string.IsNullOrEmpty(versionString) && Enum.TryParse<QueueClientOptions.ServiceVersion>(versionString, out var version))
        {
            options = new QueueClientOptions(version);
            _logger.LogInformation("QueueClient initialization with version: {Version}", versionString);
        }
        else
        {
            options = new QueueClientOptions(); 
        }

        _queueClient = new QueueClient(connectionString, queueName, options);
    }

    public async Task PublishAsync(Guid solicitudId, DateTime createdAt, CancellationToken ct)
    {
        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var message = new
            {
                Id = solicitudId,
                FechaCreacion = createdAt,
                TipoEvento = "RequestCreated"
            };

            string messageBody = JsonSerializer.Serialize(message);
            await _queueClient.SendMessageAsync(messageBody, ct);

            _logger.LogInformation("RequestCreated event published for request {Id}", solicitudId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing to Azure Queue Storage");
            throw;
        }
    }
}
