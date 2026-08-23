using CashTracer.Application.Dtos;
using CashTracer.Application.Errors;
using CashTracer.Application.Interfaces;
using CashTracer.Application.Requests;
using CashTracer.Domain.Common;
using CashTracer.Domain.Entities;
using CashTracer.Domain.Repositories;
using CashTracer.Domain.ValueObjects;

namespace CashTracer.Application.Services;

/// <summary>
/// Represents a service that handles financial transactions.
/// </summary>
/// <param name="repository">The repository to use for transaction persistence.</param>
internal sealed class TransactionService(ITransactionRepository repository)
    : ITransactionService
{
    /// <inheritdoc/>
    public async Task<Result<TransactionDto>> AddAsync(AddTransactionRequest request, CancellationToken ct = default)
    {
        var moneyResult = Money.Create(request.Currency, request.Amount);
        if (!moneyResult.IsSuccess)
        {
            return moneyResult.Error;
        }

        var creationResult = Transaction.Create(
            request.Type,
            request.Concept,
            request.Date,
            moneyResult.Value);

        if (!creationResult.IsSuccess)
        {
            return creationResult.Error;
        }

        var newTransaction = creationResult.Value;
        var insertedTransaction = await repository.AddAsync(newTransaction, ct);
        return new TransactionDto(
            insertedTransaction.Id,
            insertedTransaction.Type,
            insertedTransaction.Concept,
            insertedTransaction.Date,
            insertedTransaction.Money);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TransactionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var transactions = await repository.GetAllAsync(ct);
        return transactions
            .Select(transaction => new TransactionDto(
                transaction.Id,
                transaction.Type,
                transaction.Concept,
                transaction.Date,
                transaction.Money))
            .ToArray();
    }

    /// <inheritdoc/>
    public async Task<Result<TransactionDto>> UpdateAsync(int id, UpdateTransactionRequest request, CancellationToken ct = default)
    {
        var transaction = await repository.GetByIdAsync(id, ct);
        if (transaction is null)
        {
            return TransactionServiceErrors.TransactionNotFound(id);
        }

        Money? updatedMoney = null;
        if (request.Currency is not null || request.Amount is not null)
        {
            var currency = request.Currency ?? transaction.Money.Currency;
            var amount = request.Amount ?? transaction.Money.Amount;
            var moneyResult = Money.Create(currency, amount);
            if (!moneyResult.IsSuccess)
            {
                return moneyResult.Error;
            }

            updatedMoney = moneyResult.Value;
        }

        var updateResult = transaction.Update(request.Type, request.Concept, request.Date, updatedMoney);
        if (!updateResult.IsSuccess)
        {
            return updateResult.Error;
        }

        await repository.UpdateAsync(transaction, ct);
        return new TransactionDto(
            transaction.Id,
            transaction.Type,
            transaction.Concept,
            transaction.Date,
            transaction.Money);
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
    {
        var transaction = await repository.GetByIdAsync(id, ct);
        if (transaction is null)
        {
            return TransactionServiceErrors.TransactionNotFound(id);
        }

        await repository.DeleteAsync(id, ct);
        return Result.Success;
    }
}