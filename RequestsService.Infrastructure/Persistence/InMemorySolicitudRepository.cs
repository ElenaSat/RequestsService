using System.Collections.Concurrent;
using RequestsService.Domain.Entities;
using RequestsService.Domain.Repositories;

namespace RequestsService.Infrastructure.Persistence;

public class InMemorySolicitudRepository : ISolicitudRepository
{
    private readonly ConcurrentDictionary<Guid, Solicitud> _solicitudes = new();

    public Task AddAsync(Solicitud solicitud, CancellationToken ct = default)
    {
        _solicitudes.TryAdd(solicitud.Id, solicitud);
        return Task.CompletedTask;
    }

    public Task<Solicitud?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _solicitudes.TryGetValue(id, out var solicitud);
        return Task.FromResult(solicitud);
    }

    public Task<List<Solicitud>> GetAllAsync(CancellationToken ct = default)
    {
        var result = _solicitudes.Values.ToList();
        return Task.FromResult(result);
    }
}
