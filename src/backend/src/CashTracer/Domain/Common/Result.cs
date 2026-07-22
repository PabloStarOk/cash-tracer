using System.Diagnostics.CodeAnalysis;

namespace CashTracer.Domain.Common;

/// <summary>
/// Represents the result of an operation, which can either be successful or failed.
/// </summary>
/// <typeparam name="T">The type of the value contained in the result.</typeparam>
public sealed record Result<T>
    where T : notnull
{
    /// <summary>
    /// Gets a value indicating whether indicates whether the result is successful or not.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the value contained in the result if it is successful; otherwise, throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    public T? Value
    {
        get
        {
            return !IsSuccess
            ? throw new InvalidOperationException("Cannot access the value of a failed result.")
            : field;
        }
    }

    /// <summary>
    /// Gets the error message if the result is failed; otherwise, returns null.
    /// </summary>
    public Error? Error { get; }

    private Result(bool isSuccess, T? value, Error? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Creates a new successful result with the specified value.
    /// </summary>
    /// <param name="value">The value to be contained in the result.</param>
    /// <returns>A new successful result.</returns>
    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null);
    }

    /// <summary>
    /// Creates a new failed result with the specified error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A new failed result.</returns>
    public static Result<T> Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new Result<T>(false, default, error);
    }

    /// <summary>
    /// Defines an implicit conversion from a value of type <typeparamref name="T"/> to a successful <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    public static implicit operator Result<T>(T value) => Success(value);
}