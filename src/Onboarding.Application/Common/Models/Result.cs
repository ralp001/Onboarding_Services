// Application/Common/Models/Result.cs
namespace Onboarding.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }  // Changed from Message to Error for failures
    public string Message { get; } // Keep for successes
    public List<string> Errors { get; }

    protected Result(bool isSuccess, string error, string message, List<string> errors)
    {
        IsSuccess = isSuccess;
        Error = error;
        Message = message;
        Errors = errors;
    }

    public static Result Success(string message = "Success")
        => new Result(true, string.Empty, message, []);

    public static Result Failure(string error, List<string> errors = null)
        => new Result(false, error, string.Empty, errors ?? []);

    // These factory methods return Result<T>
    public static Result<T> Success<T>(T data, string message = "Success")
        => Result<T>.Success(data, message);

    public static Result<T> Failure<T>(string error, List<string> errors = null)
        => Result<T>.Failure(error, errors);
}

public class Result<T> : Result
{
    public T Value { get; }  // Changed from Data to Value for consistency

    internal Result(bool isSuccess, string error, string message, T value, List<string> errors)
        : base(isSuccess, error, message, errors)
    {
        Value = value;
    }

    // Add these static methods to Result<T>
    public static Result<T> Success(T value, string message = "Success")
        => new Result<T>(true, string.Empty, message, value, []);

    public static Result<T> Failure(string error, List<string> errors = null)
        => new Result<T>(false, error, string.Empty, default!, errors ?? []);
}