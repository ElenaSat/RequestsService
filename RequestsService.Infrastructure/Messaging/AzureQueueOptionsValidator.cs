using FluentValidation;

namespace RequestsService.Infrastructure.Messaging;

public class AzureQueueOptionsValidator : AbstractValidator<AzureQueueOptions>
{
    public AzureQueueOptionsValidator()
    {
        RuleFor(x => x.ConnectionString)
            .NotEmpty().WithMessage("Azure Queue Storage ConnectionString is required.");

        RuleFor(x => x.QueueName)
            .NotEmpty().WithMessage("Azure Queue Name is required.");
    }
}
