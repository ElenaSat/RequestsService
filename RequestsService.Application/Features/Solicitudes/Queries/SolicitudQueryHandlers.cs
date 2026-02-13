using MediatR;
using RequestsService.Application.Common;
using RequestsService.Application.DTOs;
using RequestsService.Domain.Repositories;

namespace RequestsService.Application.Features.Solicitudes.Queries;

public class SolicitudQueryHandlers : 
    IRequestHandler<ObtenerSolicitudQuery, Result<SolicitudResponse>>,
    IRequestHandler<ObtenerSolicitudesQuery, Result<List<SolicitudResponse>>>
{
    private readonly ISolicitudRepository _repository;

    public SolicitudQueryHandlers(ISolicitudRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SolicitudResponse>> Handle(ObtenerSolicitudQuery request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id, ct);
        if (entity == null) {
           
            return Result<SolicitudResponse>.Failure(new[] { "Solicitud no encontrada" });
        }

        var response = new SolicitudResponse(
            entity.Id, 
            entity.Name, 
            entity.Payload, 
            entity.Status, 
            entity.CreatedAt
        );

        return Result<SolicitudResponse>.Success(response);
    }

    public async Task<Result<List<SolicitudResponse>>> Handle(ObtenerSolicitudesQuery request, CancellationToken ct)
    {
        var entities = await _repository.GetAllAsync(ct);
        
        var response = entities.Select(e => new SolicitudResponse(
            e.Id, 
            e.Name, 
            e.Payload, 
            e.Status, 
            e.CreatedAt
        )).ToList();

        return Result<List<SolicitudResponse>>.Success(response);
    }
}
