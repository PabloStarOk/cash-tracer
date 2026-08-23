using CashTracer.Infrastructure.Persistence.Sqlite.Factories;

using Microsoft.Extensions.DependencyInjection;

namespace CashTracer.Infrastructure.Persistence.Sqlite;

/// <summary>
/// Extension methods for configuring SQLite persistence services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds SQLite persistence services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddSqlitePersistence(this IServiceCollection services)
    {
        services.AddSingleton<SqlConnectionFactory>();
        services.AddSingleton<ISqlConnectionFactory>(sp => sp.GetRequiredService<SqlConnectionFactory>());
        services.AddHostedService(sp => sp.GetRequiredService<SqlConnectionFactory>());
        return services;
    }
}