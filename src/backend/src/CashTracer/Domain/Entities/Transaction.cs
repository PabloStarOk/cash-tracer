using CashTracer.Domain.Common;
using CashTracer.Domain.Enums;
using CashTracer.Domain.Errors;
using CashTracer.Domain.ValueObjects;

namespace CashTracer.Domain.Entities;

/// <summary>
/// Represents a financial transaction.
/// </summary>
public sealed class Transaction
{
    /// <summary>
    /// Gets the maximum allowed length for the concept of a transaction.
    /// </summary>
    public const int MaxConceptLength = 50;

    /// <summary>
    /// Gets the unique identifier of the transaction.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the type of the transaction.
    /// </summary>
    public TransactionType Type { get; private set; }

    /// <summary>
    /// Gets the concept or description of the transaction.
    /// </summary>
    public string Concept { get; private set; }

    /// <summary>
    /// Gets the date of the transaction.
    /// </summary>
    public DateOnly Date { get; private set; }

    /// <summary>
    /// Gets the monetary value of the transaction.
    /// </summary>
    public Money Money { get; private set; }

    /// <summary>
    /// Gets the creation timestamp of the transaction.
    /// </summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets the last updated timestamp of the transaction, if applicable.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; private set; }

    private Transaction(int id, TransactionType type, string concept, DateOnly date, Money money)
    {
        Id = id;
        Type = type;
        Concept = concept;
        Date = date;
        Money = money;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    private Transaction(TransactionType type, string concept, DateOnly date, Money money)
    {
        Type = type;
        Concept = concept;
        Date = date;
        Money = money;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Transaction"/> with the specified type, concept, date, and monetary value.
    /// </summary>
    /// <param name="type">The type of the transaction.</param>
    /// <param name="concept">The concept or description of the transaction.</param>
    /// <param name="date">The date of the transaction.</param>
    /// <param name="money">The monetary value of the transaction.</param>
    /// <returns>A <see cref="Result{Transaction}"/> containing the created <see cref="Transaction"/> instance or an error.</returns>
    public static Result<Transaction> Create(TransactionType type, string concept, DateOnly date, Money money)
    {
        var validationError = ValidateConcept(concept);
        return validationError is not null
            ? Result<Transaction>.Failure(validationError)
            : new Transaction(
                type,
                concept,
                date,
                money);
    }

    /// <summary>
    /// Creates a new instance of <see cref="Transaction"/> with the specified id, type, concept, date, and monetary value.
    /// </summary>
    /// <param name="id">The id of the transaction.</param>
    /// <param name="type">the type of the transaction.</param>
    /// <param name="concept">The concept or description of the transaction.</param>
    /// <param name="date">The date of the transaction.</param>
    /// <param name="money">The monetary value of the transaction.</param>
    /// <returns>A <see cref="Result{Transaction}"/> containing the created <see cref="Transaction"/> instance or an error.</returns>
    public static Result<Transaction> CreateWithId(int id, TransactionType type, string concept, DateOnly date, Money money)
    {
        var validationError = ValidateConcept(concept);
        return validationError is not null
            ? Result<Transaction>.Failure(validationError)
            : new Transaction(
                id,
                type,
                concept,
                date,
                money);
    }

    /// <summary>
    /// Updates the transaction with the provided values.
    /// </summary>
    /// <param name="newType">The updated type, if any.</param>
    /// <param name="newConcept">The updated concept, if any.</param>
    /// <param name="newDate">The updated date, if any.</param>
    /// <param name="newMoney">The updated money value, if any.</param>
    /// <returns>The updated transaction or a validation error.</returns>
    public Result<Transaction> Update(
        TransactionType? newType = null,
        string? newConcept = null,
        DateOnly? newDate = null,
        Money? newMoney = null)
    {
        var updatedConcept = newConcept ?? Concept;
        var validationError = ValidateConcept(updatedConcept);
        if (validationError is not null)
        {
            return Result<Transaction>.Failure(validationError);
        }

        Type = newType ?? Type;
        Concept = updatedConcept;
        Date = newDate ?? Date;
        Money = newMoney ?? Money;
        UpdatedAt = DateTimeOffset.UtcNow;
        return this;
    }

    private static Error? ValidateConcept(string concept)
    {
        if (string.IsNullOrWhiteSpace(concept))
        {
            return TransactionErrors.NullOrEmptyConcept;
        }

        if (concept.Length > MaxConceptLength)
        {
            return TransactionErrors.ConceptTooLong;
        }

        return null;
    }
}