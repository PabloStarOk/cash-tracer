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

    /// <summary>
    /// Retrieves a stored transaction by id.
    /// </summary>
    /// <param name="id">The transaction identifier.</param>
    /// <param name="ct">A token to monitor the request cancellation.</param>
    /// <returns>A task containing the transaction if found; otherwise, null.</returns>
    Task<Transaction?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing transaction in the repository.
    /// </summary>
    /// <param name="transaction">The transaction with updated values.</param>
    /// <param name="ct">A token to monitor the request cancellation.</param>
    /// <returns>A task containing the updated transaction.</returns>
    Task UpdateAsync(Transaction transaction, CancellationToken ct = default);
}