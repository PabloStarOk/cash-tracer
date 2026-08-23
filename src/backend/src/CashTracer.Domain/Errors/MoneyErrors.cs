using CashTracer.Domain.Common;
using CashTracer.Domain.ValueObjects;

namespace CashTracer.Domain.Errors;

/// <summary>
/// Represents a static class containing predefined error instances related to the <see cref="Money"/> value object.
/// </summary>
public static class MoneyErrors
{
    /// <summary>
    /// Represents an error indicating that the currency is invalid, i.e., it is not a 3-letter ISO 4217 code.
    /// </summary>
    public static readonly Error InvalidCurrency = new (
        ErrorType.Validation,
        $"{MoneyErrorPrefix}.{nameof(InvalidCurrency)}",
        "Currency must be a 3-letter ISO 4217 code.");

    /// <summary>
    /// Represents an error indicating that the amount is invalid, i.e., it is zero or negative.
    /// </summary>
    public static readonly Error InvalidAmount = new (
        ErrorType.Validation,
        $"{MoneyErrorPrefix}.{nameof(InvalidAmount)}",
        "Amount cannot be zero or negative.");

    private const string MoneyErrorPrefix = nameof(Money);
}