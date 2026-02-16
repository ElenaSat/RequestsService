using Azure.Storage.Queues;

namespace RequestsService.Infrastructure.Messaging;

public class AzureQueueOptions
{
    public const string SectionName = "AzureQueueStorage";
    
    public string ConnectionString { get; init; } = null!;
    public string QueueName { get; init; } = "request-created-queue";
    public QueueClientOptions.ServiceVersion? ServiceVersion { get; init; }
}
