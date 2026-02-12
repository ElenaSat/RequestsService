using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using RequestsService.Application.Common.Interfaces;

namespace RequestsService.Infrastructure.Messaging;

public class AzureQueueStoragePublisher : IRequestCreatedPublisher
{
    private readonly QueueClient _queueClient;

    public AzureQueueStoragePublisher(IConfiguration config)
    {
        var connStr = config["AzureQueueStorage:ConnectionString"];
        var queueName = config["AzureQueueStorage:QueueName"];

        if (string.IsNullOrEmpty(connStr))
            throw new ArgumentNullException(nameof(connStr), "Azure Queue ConnectionString is missing.");
        
        if (string.IsNullOrEmpty(queueName))
            throw new ArgumentNullException(nameof(queueName), "Azure Queue Name is missing.");
        
        _queueClient = new QueueClient(connStr, queueName, new QueueClientOptions
        {
            MessageEncoding = QueueMessageEncoding.Base64 // Ensure Base64 encoding for compatibility
        });

        // Ensure queue exists (best practice for startup or catch errors here, but for simplicity we assume it might exist or fail later)
        // Usually creation is done separately or handled here.
        _queueClient.CreateIfNotExists();
    }

    public async Task PublishAsync(Guid solicitudId, DateTime createdAt, CancellationToken ct = default)
    {
        var message = new 
        { 
            EventType = "RequestCreated", 
            SolicitudId = solicitudId, 
            CreatedAt = createdAt, 
            Timestamp = DateTime.UtcNow 
        };

        var json = JsonSerializer.Serialize(message);
        // QueueClient handles Base64 encoding if configured, or we can send BinaryData
        
        await _queueClient.SendMessageAsync(BinaryData.FromString(json), cancellationToken: ct);
    }
}
