using CashTracer.Application.Interfaces;
using CashTracer.Application.Requests;

using Microsoft.AspNetCore.Mvc;

namespace CashTracer.Api.Endpoints;

/// <summary>
/// Represents the endpoint for adding a new financial transaction.
/// </summary>
public static class AddTransactionEndpoint
{
    /// <summary>
    /// Maps the endpoint for adding a new transaction to the specified route builder.
    /// </summary>
    /// <param name="routeBuilder">The route builder to map the endpoint to.</param>
    public static void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(string.Empty, HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] AddTransactionRequest request,
        [FromServices] ITransactionService service,
        CancellationToken ct)
    {
        var dto = await service.AddAsync(request, ct);
        return Results.Created(uri: string.Empty, value: dto);
    }
}