using MediatR;
using RequestsService.Application.DTOs;
using RequestsService.Domain.Common;

namespace RequestsService.Application.Features.Solicitudes.Queries;

public record ObtenerSolicitudQuery(Guid Id) : IRequest<Result<SolicitudResponse>>;

public record ObtenerSolicitudesQuery() : IRequest<Result<List<SolicitudResponse>>>;
