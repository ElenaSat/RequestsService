using FluentValidation;
using RequestsService.Application.DTOs;

namespace RequestsService.Application.Features.Solicitudes.Create;

public class CreateSolicitudValidator : AbstractValidator<CrearSolicitudRequest>
{
    public CreateSolicitudValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Payload)
            .NotEmpty().WithMessage("Payload is required.");
    }
}
