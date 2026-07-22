using CashTracer.Domain.Common;
using CashTracer.Domain.Entities;

namespace CashTracer.Domain.Errors;

/// <summary>
/// Represents a static class containing predefined error instances related to the <see cref="Transaction"/> entity.
/// </summary>
public static class TransactionErrors
{
    /// <summary>
    /// Represents an error indicating that the concept of the transaction is null or empty, which is not allowed.
    /// </summary>
    public static readonly Error NullOrEmptyConcept = new (
                ErrorType.Validation,
                $"{TransactionErrorPrefix}.{nameof(NullOrEmptyConcept)}",
                "Concept cannot be null or empty.");

    /// <summary>
    /// Represents an error indicating that the concept of the transaction is too long.
    /// </summary>
    public static readonly Error ConceptTooLong = new (
                ErrorType.Validation,
                $"{TransactionErrorPrefix}.{nameof(ConceptTooLong)}",
                $"Concept cannot be longer than {Transaction.MaxConceptLength} characters.");

    private const string TransactionErrorPrefix = nameof(Transaction);
}