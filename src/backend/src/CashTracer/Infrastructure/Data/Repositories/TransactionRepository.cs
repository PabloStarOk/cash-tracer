using System.Collections.Concurrent;

using CashTracer.Domain.Entities;
using CashTracer.Domain.Repositories;

namespace CashTracer.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a repository for managing transactions.
/// </summary>
internal sealed class TransactionRepository : ITransactionRepository
{
    private static readonly ConcurrentBag<Transaction> Transactions = [];

    /// <inheritdoc/>
    public Task<Transaction> AddAsync(Transaction transaction, CancellationToken ct = default)
    {
        var creationResult = Transaction.CreateWithId(
            Transactions.Count + 1,
            transaction.Type,
            transaction.Concept,
            transaction.Date,
            transaction.Money);

        if (!creationResult.IsSuccess)
        {
            throw new InvalidOperationException($"Failed to create transaction due to error: {creationResult.Error.Message}.");
        }

        var newTransaction = creationResult.Value;
        Transactions.Add(newTransaction);
        return Task.FromResult(newTransaction);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<Transaction>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = Transactions
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id)
            .ToArray();
        return Task.FromResult<IReadOnlyList<Transaction>>(entities);
    }
}