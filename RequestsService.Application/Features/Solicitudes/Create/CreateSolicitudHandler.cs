using FluentValidation;
using MediatR;
using RequestsService.Application.Common;
using RequestsService.Application.Common.Interfaces;
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
            return Result<SolicitudResponse>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        var entity = Solicitud.Create(request.Name, request.Payload);

        await _repository.AddAsync(entity, ct);
        await _publisher.PublishAsync(entity.Id, entity.CreatedAt, ct);

        var response = new SolicitudResponse(
            entity.Id,
            entity.Name,
            entity.Payload,
            entity.Status,
            entity.CreatedAt);

        return Result<SolicitudResponse>.Success(response);
    }
}
