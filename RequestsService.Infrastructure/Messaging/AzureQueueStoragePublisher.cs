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
    }

    public async Task PublishAsync(Guid solicitudId, DateTime createdAt, CancellationToken ct = default)
    {
        // Resiliencia: Asegura que la cola exista antes de cada publicación.
        // Si Azurite se reinicia y pierde su estado en memoria, la cola se recrea automáticamente.
        await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

        var message = new 
        { 
            EventType = "RequestCreated", 
            SolicitudId = solicitudId, 
            CreatedAt = createdAt, 
            Timestamp = DateTime.UtcNow 
        };

        var json = JsonSerializer.Serialize(message);
        
        await _queueClient.SendMessageAsync(BinaryData.FromString(json), cancellationToken: ct);
    }
}
