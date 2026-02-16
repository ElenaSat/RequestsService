using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RequestsService.Application.DTOs;
using RequestsService.Application.Features.Solicitudes.Create;
using RequestsService.Application.Features.Solicitudes.Queries;
using RequestsService.Domain.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace RequestsService.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class SolicitudesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SolicitudesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Crear nueva solicitud y notificar Azure Queue</summary>
    [HttpPost]
    [ProducesResponseType(typeof(SolicitudResponse), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [SwaggerOperation("Crea solicitud en estado Pending y publica RequestCreated")]
    public async Task<ActionResult<SolicitudResponse>> Post([FromBody] CrearSolicitudRequest request)
    {
        var command = new CrearSolicitudCommand(request);
        var result = await _mediator.Send(command);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id, version = "1.0" }, result.Value)
            : BadRequest(new ValidationProblemDetails(
                result.Errors.ToDictionary(e => "Errors", e => new[] { e })
            ));
    }

    /// <summary>Obtener solicitud por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SolicitudResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<SolicitudResponse>> GetById(Guid id)
    {
        var query = new ObtenerSolicitudQuery(id);
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess) return NotFound();

        return Ok(result.Value);
    }

    /// <summary>Lista completa de solicitudes</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<SolicitudResponse>), 200)]
    public async Task<ActionResult<List<SolicitudResponse>>> GetAll()
    {
        var query = new ObtenerSolicitudesQuery();
        var result = await _mediator.Send(query);
        return Ok(result.Value);
    }
}
