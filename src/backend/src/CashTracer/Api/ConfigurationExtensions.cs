using CashTracer.Api.Endpoints;

namespace CashTracer.Api;

/// <summary>
/// Extension methods for configuring the CashTracer API in the web application.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Configures the CashTracer API in the specified web application.
    /// </summary>
    /// <param name="app">The web application to configure the API for.</param>
    public static void ConfigureApi(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        TransactionEndpointGroup.MapGroup(app);
    }
}