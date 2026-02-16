using RequestsService.Domain.Common;

namespace RequestsService.Application.Common.Interfaces;

public interface IRequestCreatedPublisher
{
    Task<Result> PublishAsync(Guid solicitudId, DateTime createdAt, CancellationToken ct = default);
}
