using MediatR;
using RequestsService.Application.Common;
using RequestsService.Application.DTOs;

namespace RequestsService.Application.Features.Solicitudes.Queries;

public record ObtenerSolicitudQuery(Guid Id) : IRequest<Result<SolicitudResponse>>;

public record ObtenerSolicitudesQuery() : IRequest<Result<List<SolicitudResponse>>>;
