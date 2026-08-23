using System.Data.Common;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Hosting;

namespace CashTracer.Infrastructure.Persistence.Sqlite.Factories;

/// <summary>
/// Factory to open connections to the SQLite database.
/// </summary>
internal sealed class SqlConnectionFactory : IHostedService, ISqlConnectionFactory
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlConnectionFactory"/> class.
    /// </summary>
    public SqlConnectionFactory()
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = "cash-tracer.db",
            ForeignKeys = true,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Pooling = true,
        };

        _connectionString = connectionStringBuilder.ToString();
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await InitializeDatabaseAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async ValueTask<DbConnection> OpenAsync(CancellationToken ct)
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(ct);
        return connection;
    }

    private async ValueTask InitializeDatabaseAsync(CancellationToken ct)
    {
        await using var connection = await OpenAsync(ct);
        await using var setupDbCommand = connection.CreateCommand();
        setupDbCommand.CommandText =
            @"
                PRAGMA journal_mode = WAL;
                PRAGMA synchronous = NORMAL;
            ";
        await setupDbCommand.ExecuteNonQueryAsync(ct);
    }
}