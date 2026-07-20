namespace CashTracer.Domain.ValueObjects;

/// <summary>
/// Represents a monetary amount with a specific currency.
/// </summary>
/// <param name="Currency">The currency of the monetary amount.</param>
/// <param name="Amount">The amount of the monetary amount.</param>
public sealed record Money(string Currency, decimal Amount);