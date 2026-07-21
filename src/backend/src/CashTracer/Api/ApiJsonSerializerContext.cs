using System.Text.Json.Serialization;

using CashTracer.Application.Dtos;
using CashTracer.Application.Requests;
using CashTracer.Domain.ValueObjects;

namespace CashTracer.Api;

/// <summary>
/// Represents the JSON serializer context for the CashTracer API, providing serialization options and type mappings
/// for various DTOs and requests.
/// </summary>
[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    PropertyNameCaseInsensitive = true,
    UseStringEnumConverter = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip)]
[JsonSerializable(typeof(TransactionDto))]
[JsonSerializable(typeof(AddTransactionRequest))]
[JsonSerializable(typeof(Money))]
public partial class ApiJsonSerializerContext : JsonSerializerContext
{
}