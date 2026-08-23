using CashTracer.Domain.Common;
using CashTracer.Domain.Errors;

namespace CashTracer.Domain.ValueObjects;

/// <summary>
/// Represents a monetary amount with a specific currency.
/// </summary>
public sealed record Money
{
    /// <summary>
    /// Gets the currency of the monetary amount, represented as a 3-letter ISO 4217 code.
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// Gets the amount of money, which must be a non-negative decimal value.
    /// </summary>
    public decimal Amount { get; }

    private Money(string currency, decimal amount)
    {
        Currency = currency;
        Amount = amount;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Money"/> with the specified currency and amount.
    /// </summary>
    /// <param name="currency">The currency of the monetary amount.</param>
    /// <param name="amount">The amount of the monetary amount.</param>
    /// <returns>A <see cref="Result{Money}"/> containing the created <see cref="Money"/> instance or an error.</returns>
    public static Result<Money> Create(string currency, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(currency) || currency.Length is not 3)
        {
            return MoneyErrors.InvalidCurrency;
        }

        if (amount <= 0)
        {
            return MoneyErrors.InvalidAmount;
        }

        return new Money(currency, amount);
    }
}