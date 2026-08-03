using CashTracer.Domain.Enums;

namespace CashTracer.Application.Requests;

/// <summary>
/// Represents a request to update an existing financial transaction.
/// </summary>
public sealed record UpdateTransactionRequest
{
    /// <summary>
    /// Gets the <see cref="TransactionType"/> to update.
    /// </summary>
    public TransactionType? Type { get; init; }

    /// <summary>
    /// Gets the concept to update.
    /// </summary>
    public string? Concept { get; init; }

    /// <summary>
    /// Gets the transaction date to update.
    /// </summary>
    public DateOnly? Date { get; init; }

    /// <summary>
    /// Gets the currency to update.
    /// </summary>
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the amount to update.
    /// </summary>
    public decimal? Amount { get; init; }
}