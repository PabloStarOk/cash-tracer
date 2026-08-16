using System.Diagnostics.CodeAnalysis;

namespace CashTracer.Domain.Common;

/// <summary>
/// Represents the result of an operation, which can either be successful or failed.
/// </summary>
public record Result
{
    /// <summary>
    /// Gets a static instance of a successful result.
    /// </summary>
    public static readonly Result Success = new (true, default);

    /// <summary>
    /// Gets a value indicating whether indicates whether the result is successful or not.
    /// </summary>
    public virtual bool IsSuccess { get; }

    /// <summary>
    /// Gets the <see cref="Error"/> if the result is failed; otherwise, returns null.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the result is successful.</param>
    /// <param name="error">The error to be contained in the result, if the result is failed.</param>
    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a new failed result with the specified <see cref="Error"/>.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A new failed result.</returns>
    public static Result Failure(Error error)
    {
        return new Result(isSuccess: false, error);
    }

    /// <summary>
    /// Defines an implicit conversion from an <see cref="Error"/> to a failed <see cref="Result"/>.
    /// </summary>
    /// <param name="value">The error to be converted.</param>
    public static implicit operator Result(Error value) => Failure(value);
}

/// <summary>
/// Represents the result of an operation, which can either be successful or failed.
/// </summary>
/// <typeparam name="T">The type of the value contained in the result.</typeparam>
public sealed record Result<T> : Result
    where T : notnull
{
    /// <summary>
    /// Gets a value indicating whether indicates whether the result is successful or not.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    public override bool IsSuccess => base.IsSuccess;

    /// <summary>
    /// Gets the value contained in the result if it is successful; otherwise, throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    public T? Value
    {
        get
        {
            return IsSuccess
            ? field
            : throw new InvalidOperationException("Cannot access the value of a failed result.");
        }
    }

    private Result(bool isSuccess, T? value, Error error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new successful result with the specified value.
    /// </summary>
    /// <param name="value">The value to be contained in the result.</param>
    /// <returns>A new successful result.</returns>
    public static new Result<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Result<T>(isSuccess: true, value, error: default);
    }

    /// <summary>
    /// Creates a new failed result with the specified error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A new failed result.</returns>
    public static new Result<T> Failure(Error error)
    {
        return new Result<T>(isSuccess: false, value: default, error);
    }

    /// <summary>
    /// Defines an implicit conversion from a value of type <typeparamref name="T"/> to a successful <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Defines an implicit conversion from an <see cref="Error"/> to a failed <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The error to be converted.</param>
    public static implicit operator Result<T>(Error error) => Failure(error);
}