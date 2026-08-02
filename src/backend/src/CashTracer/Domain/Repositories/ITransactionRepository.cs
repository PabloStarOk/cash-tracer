using CashTracer.Domain.Entities;

namespace CashTracer.Domain.Repositories;

/// <summary>
/// Repository for transaction persistence.
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// Adds a new transaction to the repository.
    /// </summary>
    /// <param name="transaction">The transaction to add.</param>
    /// <param name="ct">A token to monitor the request cancellation.</param>
    /// <returns>A task containing the added transaction.</returns>
    Task<Transaction> AddAsync(Transaction transaction, CancellationToken ct = default);

    /// <summary>
    /// Retrieves all stored transactions.
    /// </summary>
    /// <param name="ct">A token to monitor the request cancellation.</param>
    /// <returns>A task containing all stored transactions.</returns>
    Task<IReadOnlyList<Transaction>> GetAllAsync(CancellationToken ct = default);
}