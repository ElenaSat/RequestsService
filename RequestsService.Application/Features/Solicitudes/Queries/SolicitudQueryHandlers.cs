using MediatR;
using RequestsService.Application.Common;
using RequestsService.Domain.Common;
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
        var result = await _repository.GetByIdAsync(request.Id, ct);
        if (result.IsFailure) {
            return Result<SolicitudResponse>.Failure(result.Errors);
        }

        var entity = result.Value!;
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
        var result = await _repository.GetAllAsync(ct);
        if (result.IsFailure)
        {
            return Result<List<SolicitudResponse>>.Failure(result.Errors);
        }
        
        var response = result.Value!.Select(e => new SolicitudResponse(
            e.Id, 
            e.Name, 
            e.Payload, 
            e.Status, 
            e.CreatedAt
        )).ToList();

        return Result<List<SolicitudResponse>>.Success(response);
    }
}
