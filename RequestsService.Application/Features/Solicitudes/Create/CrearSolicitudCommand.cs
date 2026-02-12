using MediatR;
using RequestsService.Application.Common;
using RequestsService.Application.DTOs;

namespace RequestsService.Application.Features.Solicitudes.Create;

public record CrearSolicitudCommand(CrearSolicitudRequest Request) : IRequest<Result<SolicitudResponse>>;
