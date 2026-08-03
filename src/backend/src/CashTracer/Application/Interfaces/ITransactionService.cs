using CashTracer.Application.Dtos;
using CashTracer.Application.Requests;
using CashTracer.Domain.Common;

namespace CashTracer.Application.Interfaces;

/// <summary>
/// Defines the contract for a service that handles financial transactions.
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Adds a new transaction based on the provided request.
    /// </summary>
    /// <param name="request">The request containing the transaction details.</param>
    /// <param name="ct">A token to monitor the request cancellation.</param>
    /// <returns>A task containing the added transaction.</returns>
    Task<Result<TransactionDto>> AddAsync(AddTransactionRequest request, CancellationToken ct = default);

    /// <summary>
    /// Retrieves all stored transactions.
    /// </summary>
    /// <param name="ct">A token to monitor the request cancellation.</param>
    /// <returns>A task containing all stored transactions.</returns>
    Task<IReadOnlyList<TransactionDto>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Updates an existing transaction using the provided request.
    /// </summary>
    /// <param name="id">The transaction identifier.</param>
    /// <param name="request">The request containing optional fields to update.</param>
    /// <param name="ct">A token to monitor the request cancellation.</param>
    /// <returns>A task containing the updated transaction.</returns>
    Task<Result<TransactionDto>> UpdateAsync(int id, UpdateTransactionRequest request, CancellationToken ct = default);
}