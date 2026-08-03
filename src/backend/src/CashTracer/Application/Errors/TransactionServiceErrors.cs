using CashTracer.Domain.Common;

namespace CashTracer.Application.Errors;

/// <summary>
/// Represents errors returned by transaction application service operations.
/// </summary>
public static class TransactionServiceErrors
{
    private const string TransactionServiceErrorPrefix = "TransactionService";

    /// <summary>
    /// Creates an error indicating that the transaction to update was not found.
    /// </summary>
    /// <param name="transactionId">The transaction identifier.</param>
    /// <returns>A not-found error.</returns>
    public static Error TransactionNotFound(int transactionId)
    {
        return new Error(
            ErrorType.NotFound,
            $"{TransactionServiceErrorPrefix}.{nameof(TransactionNotFound)}",
            $"Transaction with id '{transactionId}' was not found.");
    }
}