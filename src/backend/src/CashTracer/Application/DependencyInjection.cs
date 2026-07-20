using CashTracer.Application.Interfaces;
using CashTracer.Application.Services;

namespace CashTracer.Application;

/// <summary>
/// Extension methods for configuring application services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddTransient<ITransactionService, TransactionService>();
    }
}