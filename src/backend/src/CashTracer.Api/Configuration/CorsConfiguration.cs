namespace CashTracer.Api.Configuration;

/// <summary>
/// Configuration for CORS.
/// </summary>
public sealed record CorsConfiguration
{
    /// <summary>
    /// The name of the section in the configuration file.
    /// </summary>
    public const string SectionName = "Cors";

    /// <summary>
    /// Gets or sets the allowed origins for CORS.
    /// </summary>
    public string[] AllowedOrigins { get; set; } = [];
}