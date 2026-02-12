using RequestsService.Domain.Entities;

namespace RequestsService.Domain.Repositories;

public interface ISolicitudRepository
{
    Task AddAsync(Solicitud solicitud, CancellationToken ct = default);
    Task<Solicitud?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Solicitud>> GetAllAsync(CancellationToken ct = default);
}
