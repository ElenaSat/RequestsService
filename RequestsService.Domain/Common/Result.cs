namespace RequestsService.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string[] Errors { get; }
    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, string[] errors)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? Array.Empty<string>();
    }

    public static Result Success() => new Result(true, Array.Empty<string>());
    public static Result Failure(string[] errors) => new Result(false, errors);
    public static Result Failure(string error) => new Result(false, new[] { error });
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, string[] errors) : base(isSuccess, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new Result<T>(true, value, Array.Empty<string>());
    public static new Result<T> Failure(string[] errors) => new Result<T>(false, default, errors);
    public static new Result<T> Failure(string error) => new Result<T>(false, default, new[] { error });
}
