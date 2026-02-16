using MediatR;
using RequestsService.Application.DTOs;
using RequestsService.Domain.Common;

namespace RequestsService.Application.Features.Solicitudes.Create;

public record CrearSolicitudCommand(CrearSolicitudRequest Request) : IRequest<Result<SolicitudResponse>>;
