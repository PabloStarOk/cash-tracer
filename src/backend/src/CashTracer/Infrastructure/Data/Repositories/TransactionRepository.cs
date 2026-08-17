using System.Collections.Concurrent;

using CashTracer.Domain.Entities;
using CashTracer.Domain.Repositories;

namespace CashTracer.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a repository for managing transactions.
/// </summary>
internal sealed class TransactionRepository : ITransactionRepository
{
    private static readonly ConcurrentDictionary<int, Transaction> Transactions = [];
    private static int _nextTransactionId;

    /// <inheritdoc/>
    public Task<Transaction> AddAsync(Transaction transaction, CancellationToken ct = default)
    {
        var newId = Interlocked.Increment(ref _nextTransactionId);
        var creationResult = Transaction.CreateWithId(
            newId,
            transaction.Type,
            transaction.Concept,
            transaction.Date,
            transaction.Money);

        if (!creationResult.IsSuccess)
        {
            throw new InvalidOperationException($"Failed to create transaction due to error: {creationResult.Error.Message}.");
        }

        var newTransaction = creationResult.Value;

        if (!Transactions.TryAdd(newTransaction.Id, newTransaction))
        {
            throw new InvalidOperationException($"Transaction with id '{newTransaction.Id}' already exists.");
        }

        return Task.FromResult(newTransaction);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<Transaction>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = Transactions.Values
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id)
            .ToArray();
        return Task.FromResult<IReadOnlyList<Transaction>>(entities);
    }

    /// <inheritdoc/>
    public Task<Transaction?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        Transactions.TryGetValue(id, out var transaction);
        return Task.FromResult(transaction);
    }

    /// <inheritdoc/>
    public Task UpdateAsync(Transaction transaction, CancellationToken ct = default)
    {
        if (!Transactions.ContainsKey(transaction.Id))
        {
            throw new InvalidOperationException($"Transaction with id '{transaction.Id}' was not found.");
        }

        Transactions[transaction.Id] = transaction;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task DeleteAsync(int id, CancellationToken ct = default)
    {
        return Transactions.TryRemove(id, out _)
            ? Task.CompletedTask
            : throw new InvalidOperationException($"Transaction with id '{id}' was not found.");
    }
}