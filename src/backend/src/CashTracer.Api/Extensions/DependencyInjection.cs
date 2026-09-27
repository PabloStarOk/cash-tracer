using System.Text.Json;
using System.Text.Json.Serialization;

using CashTracer.Api.Configuration;
using CashTracer.Api.Serialization;
using CashTracer.Domain.Enums;

namespace CashTracer.Api.Extensions;

/// <summary>
/// Extension methods for configuring API services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds API services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configuration">The configuration.</param>
    public static void AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        AddCors(services, configuration);
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Add(ApiJsonSerializerContext.Default);
            JsonStringEnumConverter<TransactionType> transactionTypeConverter = new (JsonNamingPolicy.CamelCase);
            options.SerializerOptions.Converters.Add(transactionTypeConverter);
        });
        services.AddProblemDetails();
        services.AddOpenApi();
    }

    private static void AddCors(IServiceCollection services, IConfiguration configuration)
    {
        var corsConfig = configuration.GetRequiredSection(CorsConfiguration.SectionName).Get<CorsConfiguration>();
        ArgumentNullException.ThrowIfNull(corsConfig);
        services.AddCors(options =>
            options.AddDefaultPolicy(policy =>
                policy.WithOrigins(corsConfig.AllowedOrigins).AllowAnyHeader().AllowAnyMethod()));
    }
}