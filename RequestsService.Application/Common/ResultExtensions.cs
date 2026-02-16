using FluentValidation.Results;
using RequestsService.Domain.Common;

namespace RequestsService.Application.Common;

public static class ResultExtensions
{
    public static Result<T> ToResult<T>(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            return Result<T>.Failure("Validation failed but validation result is valid. This should not happen.");
        }

        return Result<T>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
    }

    public static Result ToResult(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            return Result.Failure("Validation failed but validation result is valid. This should not happen.");
        }

        return Result.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
    }
}
