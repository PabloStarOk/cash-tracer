namespace CashTracer.Domain.Common;

/// <summary>
/// Represents the type of an error that occurred during the execution of an operation.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Indicates that the error is related to validation, such as invalid input or business rule violations.
    /// </summary>
    Validation,

    /// <summary>
    /// Indicates that the requested resource does not exist.
    /// </summary>
    NotFound,
}