using System.Globalization;

using CashTracer.Domain.Entities;
using CashTracer.Domain.Repositories;
using CashTracer.Infrastructure.Persistence.Sqlite.Factories;
using CashTracer.Infrastructure.Persistence.Sqlite.Models;

using Dapper;

namespace CashTracer.Infrastructure.Persistence.Sqlite.Repositories;

/// <summary>
/// Repository implementation for <see cref="Transaction"/> entities.
/// </summary>
/// <param name="connectionFactory">The factory to create SQLite database connections.</param>
internal sealed class TransactionRepository(ISqlConnectionFactory connectionFactory)
    : ITransactionRepository
{
    private const string AddSql =
        @"
        INSERT INTO transactions(type, concept, date, currency, amount, created_at, updated_at)
        VALUES (@type, @concept, @date, @currency, @amount, @created_at, @updated_at)
        RETURNING id
        ";

    private const string DeleteSql =
        @"
        DELETE FROM transactions WHERE id = @id
        ";

    private const string GetAllSql =
        @"
        SELECT id, type, concept, date, currency, amount, created_at, updated_at FROM transactions
        ";

    private const string GetByIdSql =
        @"
        SELECT id, type, concept, date, currency, amount, created_at, updated_at FROM transactions WHERE id = @id
        ";

    private const string UpdateSql =
        @"
        UPDATE transactions
        SET type = @type, concept = @concept, date = @date, currency = @currency, amount = @amount, updated_at = @updated_at
        WHERE id = @id
        ";

    /// <inheritdoc />
    public async Task<int> AddAsync(Transaction transaction, CancellationToken ct = default)
    {
        await using var connection = await connectionFactory.OpenAsync(ct);
        return await connection.ExecuteScalarAsync<int>(
            AddSql,
            new
            {
                type = transaction.Type,
                concept = transaction.Concept,
                date = transaction.Date,
                currency = transaction.Money.Currency,
                amount = transaction.Money.Amount,
                created_at = transaction.CreatedAt.ToString("O", CultureInfo.InvariantCulture),
                updated_at = transaction.UpdatedAt?.ToString("O", CultureInfo.InvariantCulture),
            });
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await using var connection = await connectionFactory.OpenAsync(ct);
        await connection.ExecuteAsync(DeleteSql, new { id });
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Transaction>> GetAllAsync(CancellationToken ct = default)
    {
        await using var connection = await connectionFactory.OpenAsync(ct);
        IEnumerable<TransactionDbModel> dbModels = await connection.QueryAsync<TransactionDbModel>(GetAllSql);
        return dbModels.Select(t => t.ToDomainEntity()).ToList();
    }

    /// <inheritdoc />
    public async Task<Transaction?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var connection = await connectionFactory.OpenAsync(ct);
        TransactionDbModel? dbModel = await connection.QuerySingleOrDefaultAsync<TransactionDbModel>(
            GetByIdSql,
            new { id });
        return dbModel?.ToDomainEntity();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Transaction transaction, CancellationToken ct = default)
    {
        await using var connection = await connectionFactory.OpenAsync(ct);
        await connection.ExecuteAsync(
            UpdateSql,
            new
            {
                id = transaction.Id,
                type = transaction.Type,
                concept = transaction.Concept,
                date = transaction.Date,
                currency = transaction.Money.Currency,
                amount = transaction.Money.Amount,
                updated_at = transaction.UpdatedAt?.ToString("O", CultureInfo.InvariantCulture),
            });
    }
}