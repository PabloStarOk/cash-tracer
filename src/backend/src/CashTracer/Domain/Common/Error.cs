namespace CashTracer.Domain.Common;

/// <summary>
/// Represents an error that occurred during the execution of an operation.
/// </summary>
/// <param name="Type">The type of the error.</param>
/// <param name="Code">The code of the error.</param>
/// <param name="Message">The message of the error.</param>
public sealed record Error(ErrorType Type, string Code, string Message);