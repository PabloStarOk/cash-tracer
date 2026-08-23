using System.Data.Common;
using System.Reflection;

using CashTracer.Infrastructure.Persistence.Sqlite.Factories;

using Dapper;
using Dapper.Transaction;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

[module: DapperAot]

namespace CashTracer.Infrastructure.Persistence.Sqlite.Migrations;

/// <summary>
/// Implementation to apply SQLite migrations to the database.
/// </summary>
/// <param name="logger">Service for logging.</param>
/// <param name="connectionFactory">Factory to create database connections.</param>
internal sealed class SqlMigrator(
    ILogger<SqlMigrator> logger,
    ISqlConnectionFactory connectionFactory)
    : IHostedService
{
    private const string ExistMigrationsTableSql = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='migrations'";
    private const string CreateMigrationsTableSql = "CREATE TABLE migrations(name TEXT NOT NULL)";
    private const string GetLatestMigrationSql = "SELECT name FROM migrations ORDER BY Name DESC LIMIT 1";
    private const string InsertMigrationSql = "INSERT INTO migrations VALUES (@name)";

    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
    private static readonly string UpMigrationPrefix = $"{Assembly.GetName().Name}.Migrations.Scripts.Up";

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        await EnsureMigrationsTableIsCreatedAsync(connection);
        string? latestMigrationName = await connection.ExecuteScalarAsync<string>(GetLatestMigrationSql);
        string[] pendingMigrationNames = GetPendingMigrations(latestMigrationName);
        if (pendingMigrationNames.Length is 0)
        {
            return;
        }

        await ApplyMigrationsAsync(connection, pendingMigrationNames, cancellationToken);
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static async ValueTask<string> LoadMigrationSqlAsync(string migrationName, CancellationToken ct)
    {
        await using Stream stream =
            Assembly.GetManifestResourceStream(migrationName)
            ?? throw new InvalidOperationException($"Could not read manifest resource with name '{migrationName}'");
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(ct);
    }

    private static string[] GetPendingMigrations(string? latestMigrationName)
    {
        string[] allMigrations = Assembly
            .GetManifestResourceNames()
            .Where(name => name.StartsWith(UpMigrationPrefix))
            .Order()
            .ToArray();

        if (latestMigrationName is null)
        {
            return allMigrations;
        }

        int latestMigrationIndex = Array.IndexOf(allMigrations, latestMigrationName);
        if (latestMigrationIndex is -1)
        {
            throw new InvalidOperationException("Could not find latest applied migration in the embedded scripts");
        }

        return allMigrations.Skip(latestMigrationIndex + 1).ToArray();
    }

    private async ValueTask EnsureMigrationsTableIsCreatedAsync(DbConnection connection)
    {
        int migrationTableCount = await connection.ExecuteScalarAsync<int>(ExistMigrationsTableSql);
        bool isCreated = migrationTableCount > 0;
        if (isCreated)
        {
            return;
        }

        await connection.ExecuteAsync(CreateMigrationsTableSql);
        logger.LogDebug("Migrations table created");
    }

    private async Task ApplyMigrationsAsync(
        DbConnection connection,
        string[] migrationNames,
        CancellationToken ct)
    {
        await using DbTransaction transaction = await connection.BeginTransactionAsync(ct);
        foreach (var migrationName in migrationNames)
        {
            string migrationSql = await LoadMigrationSqlAsync(migrationName, ct);
            logger.LogTrace("Applying migration '{Name}' SQL:\n{Sql}", migrationName, migrationSql);
            await connection.ExecuteAsync(migrationSql, transaction: transaction);
            await connection.ExecuteAsync(InsertMigrationSql, new { name = migrationName }, transaction);
        }

        await transaction.CommitAsync(ct);
        logger.LogInformation("New migrations applied: {MigrationNames}", migrationNames);
    }
}