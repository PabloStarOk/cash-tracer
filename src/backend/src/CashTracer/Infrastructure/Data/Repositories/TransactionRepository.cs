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
        var newTransaction = new Transaction
        {
            Id = Transactions.Count + 1,
            Type = transaction.Type,
            Concept = transaction.Concept,
            Date = transaction.Date,
            Money = transaction.Money,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        Transactions.Add(newTransaction);
        return Task.FromResult(newTransaction);
    }
}