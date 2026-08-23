using CashTracer.Domain.Repositories;
using CashTracer.Infrastructure.Data.Repositories;

using Microsoft.Extensions.DependencyInjection;

namespace CashTracer.Infrastructure;

/// <summary>
/// Extension methods for configuring infrastructure services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITransactionRepository, TransactionRepository>();
    }
}