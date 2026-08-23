using System.Data.Common;

namespace CashTracer.Infrastructure.Persistence.Sqlite.Factories;

/// <summary>
/// Factory abstraction to open SQLite connections.
/// </summary>
internal interface ISqlConnectionFactory
{
    /// <summary>
    /// Opens a connection to the SQLite database.
    /// </summary>
    /// <param name="ct">The token to monitor for cancellation requests.</param>
    /// <returns>An opened database connection.</returns>
    ValueTask<DbConnection> OpenAsync(CancellationToken ct);
}
