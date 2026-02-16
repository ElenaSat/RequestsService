using RequestsService.Domain.Common;
using RequestsService.Domain.Entities;

namespace RequestsService.Domain.Repositories;

public interface ISolicitudRepository
{
    Task<Result> AddAsync(Solicitud solicitud, CancellationToken ct = default);
    Task<Result<Solicitud>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<List<Solicitud>>> GetAllAsync(CancellationToken ct = default);
}
