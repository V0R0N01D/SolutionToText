namespace SolutionToText.Models;

public class Result
{
    public bool IsSuccess { get; }
    public string? Message { get; }

    protected Result(bool isSuccess, string? message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static Result Success() => new Result(true, null);

    public static Result Failure(string message) => new Result(false, message);
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, string? message, T? value)
        : base(isSuccess, message)
    {
        Value = value;
    }

    public static Result<T> Success(T content)
        => new Result<T>(true, null, content);

    public static Result<T> Failure(string message, T? content = default)
        => new Result<T>(false, message, content);
}