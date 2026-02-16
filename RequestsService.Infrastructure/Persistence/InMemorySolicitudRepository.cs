using System.Collections.Concurrent;
using RequestsService.Domain.Common;
using RequestsService.Domain.Entities;
using RequestsService.Domain.Repositories;

namespace RequestsService.Infrastructure.Persistence;

public class InMemorySolicitudRepository : ISolicitudRepository
{
    private readonly ConcurrentDictionary<Guid, Solicitud> _solicitudes = new();

    public Task<Result> AddAsync(Solicitud solicitud, CancellationToken ct = default)
    {
        _solicitudes.TryAdd(solicitud.Id, solicitud);
        return Task.FromResult(Result.Success());
    }

    public Task<Result<Solicitud>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        if (_solicitudes.TryGetValue(id, out var solicitud))
        {
            return Task.FromResult(Result<Solicitud>.Success(solicitud));
        }
        return Task.FromResult(Result<Solicitud>.Failure("Solicitud no encontrada"));
    }

    public Task<Result<List<Solicitud>>> GetAllAsync(CancellationToken ct = default)
    {
        var result = _solicitudes.Values.ToList();
        return Task.FromResult(Result<List<Solicitud>>.Success(result));
    }
}
