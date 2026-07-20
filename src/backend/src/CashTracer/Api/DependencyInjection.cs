namespace CashTracer.Api;

/// <summary>
/// Extension methods for configuring API services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds API services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    public static void AddApi(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.TypeInfoResolverChain.Add(ApiJsonSerializerContext.Default));

        services.AddOpenApi();
    }
}