namespace RequestsService.Application.Common.Interfaces;

public interface IRequestCreatedPublisher
{
    Task PublishAsync(Guid solicitudId, DateTime createdAt, CancellationToken ct = default);
}
