using RequestsService.Domain.Enums;

namespace RequestsService.Application.DTOs;

public record CrearSolicitudRequest(string Name, string Payload);

public record SolicitudResponse(Guid Id, string Name, string Payload, SolicitudStatus Status, DateTime CreatedAt);
