namespace SolutionToText.Models;

/// <summary>
/// Represents the result of an operation, indicating whether it was successful and providing an error message.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }
    
    /// <summary>
    /// Gets error message that describes the failure result of the operation.
    /// </summary>
    public string? ErrorMessage { get; }
    
    
    protected Result(bool isSuccess, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A <see cref="Result"/> instance indicating success.</returns>
    public static Result Success() => new Result(true, null);

    /// <summary>
    /// Creates a failure result with the specified error errorMessage.
    /// </summary>
    /// <param name="errorMessage">An error message describing the failure.</param>
    /// <returns>A <see cref="Result"/> instnce indicating failure.</returns>
    public static Result Failure(string errorMessage) => new Result(false, errorMessage);
}

/// <summary>
/// Represents the result of an operation that returns a value.
/// </summary>
/// <typeparam name="T">Type of the value returned by the operation.</typeparam>
public sealed class Result<T> : Result
{
    /// <summary>
    /// Gets the value returned by the operation.
    /// </summary>
    public T? Value { get; }
    
    
    private Result(bool isSuccess, string? errorMessage, T? value)
        : base(isSuccess, errorMessage)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <param name="content">The value returned by the operation.</param>
    /// <returns>A <see cref="Result{T}"/> instance indicating success with the provided value.</returns>
    public static Result<T> Success(T content)
        => new Result<T>(true, null, content);

    /// <summary>
    /// Creates a failure result with the specified error errorMessage and an optional value.
    /// </summary>
    /// <param name="errorMessage">An error message describing the failure.</param>
    /// <param name="content">An optional value associated with the failure.</param>
    /// <returns>A <see cref="Result{T}"/> instance indicating failure.</returns>
    public static Result<T> Failure(string errorMessage, T? content = default)
        => new Result<T>(false, errorMessage, content);
}