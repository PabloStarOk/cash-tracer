using CashTracer.Application.Dtos;
using CashTracer.Application.Interfaces;
using CashTracer.Application.Requests;
using CashTracer.Domain.Entities;
using CashTracer.Domain.Repositories;

namespace CashTracer.Application.Services;

/// <summary>
/// Represents a service that handles financial transactions.
/// </summary>
/// <param name="repository">The repository to use for transaction persistence.</param>
internal sealed class TransactionService(ITransactionRepository repository)
    : ITransactionService
{
    /// <inheritdoc/>
    public async Task<TransactionDto> AddAsync(AddTransactionRequest request, CancellationToken ct = default)
    {
        var newTransaction = new Transaction
        {
            Type = request.Type,
            Concept = request.Concept,
            Date = request.Date,
            Money = request.Money,
        };

        var insertedTransaction = await repository.AddAsync(newTransaction, ct);
        return new TransactionDto(
            insertedTransaction.Id,
            request.Type,
            request.Concept,
            request.Date,
            request.Money);
    }
}