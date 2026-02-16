using FluentValidation;
using MediatR;
using RequestsService.Application.Common;
using RequestsService.Application.Common.Interfaces;
using RequestsService.Domain.Common;
using RequestsService.Application.DTOs;
using RequestsService.Domain.Entities;
using RequestsService.Domain.Repositories;

namespace RequestsService.Application.Features.Solicitudes.Create;

public class CreateSolicitudHandler : IRequestHandler<CrearSolicitudCommand, Result<SolicitudResponse>>
{
    private readonly ISolicitudRepository _repository;
    private readonly IRequestCreatedPublisher _publisher;
    private readonly IValidator<CrearSolicitudRequest> _validator;

    public CreateSolicitudHandler(
        ISolicitudRepository repository,
        IRequestCreatedPublisher publisher,
        IValidator<CrearSolicitudRequest> validator)
    {
        _repository = repository;
        _publisher = publisher;
        _validator = validator;
    }

    public async Task<Result<SolicitudResponse>> Handle(CrearSolicitudCommand command, CancellationToken ct)
    {
        var request = command.Request;
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return validationResult.ToResult<SolicitudResponse>();
        }

        var solicitudResult = Solicitud.Create(request.Name, request.Payload);
        if (solicitudResult.IsFailure)
        {
            return Result<SolicitudResponse>.Failure(solicitudResult.Errors);
        }

        var entity = solicitudResult.Value!;

        var repoResult = await _repository.AddAsync(entity, ct);
        if (repoResult.IsFailure)
        {
            return Result<SolicitudResponse>.Failure(repoResult.Errors);
        }

        var publishResult = await _publisher.PublishAsync(entity.Id, entity.CreatedAt, ct);
        if (publishResult.IsFailure)
        {
            // Note: In a more complex system, we might want to handle this differently (e.g., outbox pattern)
            // but for now, we return the failure.
            return Result<SolicitudResponse>.Failure(publishResult.Errors);
        }

        var response = new SolicitudResponse(
            entity.Id,
            entity.Name,
            entity.Payload,
            entity.Status,
            entity.CreatedAt);

        return Result<SolicitudResponse>.Success(response);
    }
}
