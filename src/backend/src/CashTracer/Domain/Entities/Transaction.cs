using CashTracer.Domain.Enums;
using CashTracer.Domain.ValueObjects;

namespace CashTracer.Domain.Entities;

/// <summary>
/// Represents a financial transaction.
/// </summary>
public sealed class Transaction
{
    /// <summary>
    /// Gets the unique identifier of the transaction.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the type of the transaction.
    /// </summary>
    public TransactionType Type { get; init; }

    /// <summary>
    /// Gets the concept or description of the transaction.
    /// </summary>
    public required string Concept { get; init; }

    /// <summary>
    /// Gets the date of the transaction.
    /// </summary>
    public DateOnly Date { get; init; }

    /// <summary>
    /// Gets the monetary value of the transaction.
    /// </summary>
    public required Money Money { get; init; }

    /// <summary>
    /// Gets the creation timestamp of the transaction.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets the last updated timestamp of the transaction, if applicable.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}