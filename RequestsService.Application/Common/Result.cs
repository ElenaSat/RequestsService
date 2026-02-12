namespace RequestsService.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string[] Errors { get; }

    private Result(bool isSuccess, T? value, string[] errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors;
    }

    public static Result<T> Success(T value) => new Result<T>(true, value, Array.Empty<string>());
    public static Result<T> Failure(string[] errors) => new Result<T>(false, default, errors);
}
