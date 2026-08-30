using System.Globalization;

using CashTracer.Domain.Entities;
using CashTracer.Domain.Enums;
using CashTracer.Domain.ValueObjects;

namespace CashTracer.Infrastructure.Persistence.Sqlite.Models;

/// <summary>
/// The SQLite database model for <see cref="Transaction"/> entity.
/// </summary>
public sealed record TransactionDbModel
{
    /// <summary>
    /// Gets the ID of the transaction.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets the type of the transaction.
    /// </summary>
    public required TransactionType Type { get; init; }

    /// <summary>
    /// Gets the concept of the transaction.
    /// </summary>
    public required string Concept { get; init; }

    /// <summary>
    /// Gets the date of the transaction.
    /// </summary>
    public required DateOnly Date { get; init; }

    /// <summary>
    /// Gets the currency of the transaction.
    /// </summary>
    public required string Currency { get; init; }

    /// <summary>
    /// Gets the amount of the transaction.
    /// </summary>
    public required decimal Amount { get; init; }

    /// <summary>
    /// Gets the date and time when the transaction was created in ISO-8601.
    /// </summary>
    public required string CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time of the last time the transaction was updated in ISO-8601.
    /// </summary>
    public string? UpdatedAt { get; init; }

    /// <summary>
    /// Converts this instance into an <see cref="Transaction"/> entity.
    /// </summary>
    /// <returns>A <see cref="Transaction"/> entity.</returns>
    public Transaction ToDomainEntity()
    {
        var money = Money.Reconstruct(Currency, Amount);
        var createdAt = DateTimeOffset.ParseExact(CreatedAt, "O", CultureInfo.InvariantCulture);
        DateTimeOffset? updatedAt = UpdatedAt is null
            ? null
            : DateTimeOffset.ParseExact(UpdatedAt, "O", CultureInfo.InvariantCulture);
        return Transaction.Rehydrate(Id, Type, Concept, Date, money, createdAt, updatedAt);
    }
}