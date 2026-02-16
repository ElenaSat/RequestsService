using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RequestsService.Application.Common.Interfaces;
using RequestsService.Domain.Common;
using System.Text.Json;

namespace RequestsService.Infrastructure.Messaging;

public class AzureQueueStoragePublisher : IRequestCreatedPublisher
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<AzureQueueStoragePublisher> _logger;

    public AzureQueueStoragePublisher(
        IOptions<AzureQueueOptions> options, 
        ILogger<AzureQueueStoragePublisher> logger)
    {
        _logger = logger;
        var settings = options.Value;

        var clientOptions = settings.ServiceVersion.HasValue 
            ? new QueueClientOptions(settings.ServiceVersion.Value) 
            : new QueueClientOptions();

        _queueClient = new QueueClient(settings.ConnectionString, settings.QueueName, clientOptions);
        
        if (settings.ServiceVersion.HasValue)
        {
            _logger.LogInformation("QueueClient initialization with version: {Version}", settings.ServiceVersion.Value);
        }
    }

    public async Task<Result> PublishAsync(Guid solicitudId, DateTime createdAt, CancellationToken ct)
    {
        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var eventData = new
            {
                Id = solicitudId,
                FechaCreacion = createdAt,
                TipoEvento = "RequestCreated"
            };

            string messageBody = JsonSerializer.Serialize(eventData);
            await _queueClient.SendMessageAsync(messageBody, ct);

            _logger.LogInformation("RequestCreated event published for request {Id}", solicitudId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing to Azure Queue Storage for request {Id}", solicitudId);
            return Result.Failure($"Error publishing event: {ex.Message}");
        }
    }
}
