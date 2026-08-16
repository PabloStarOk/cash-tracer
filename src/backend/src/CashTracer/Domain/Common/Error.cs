namespace CashTracer.Domain.Common;

/// <summary>
/// Represents an error that occurred during the execution of an operation.
/// </summary>
/// <param name="type">The type of the error.</param>
/// <param name="code">The code of the error.</param>
/// <param name="message">The message of the error.</param>
public readonly struct Error(ErrorType type, string code, string message)
{
    /// <summary>
    /// Gets the type of the error.
    /// </summary>
    public ErrorType Type { get; } = type;

    /// <summary>
    /// Gets the code of the error.
    /// </summary>
    public string Code { get; } = code;

    /// <summary>
    /// Gets the message of the error.
    /// </summary>
    public string Message { get; } = message;
}