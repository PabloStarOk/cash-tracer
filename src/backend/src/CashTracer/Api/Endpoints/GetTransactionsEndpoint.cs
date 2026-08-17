using CashTracer.Application.Dtos;
using CashTracer.Application.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace CashTracer.Api.Endpoints;

/// <summary>
/// Represents the endpoint for retrieving stored financial transactions.
/// </summary>
public static class GetTransactionsEndpoint
{
    /// <summary>
    /// Maps the endpoint for retrieving transactions to the specified route builder.
    /// </summary>
    /// <param name="routeBuilder">The route builder to map the endpoint to.</param>
    public static void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet(string.Empty, HandleAsync)
            .WithSummary("Get all transactions")
            .WithDescription("Retrieves all stored financial transactions.")
            .Produces<IReadOnlyList<TransactionDto>>(StatusCodes.Status200OK);
    }

    /// <summary>
    /// Retrieves all stored transactions.
    /// </summary>
    /// <param name="service">The transaction service.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>All stored transactions.</returns>
    public static async Task<IResult> HandleAsync(
        [FromServices] ITransactionService service,
        CancellationToken ct)
    {
        var transactions = await service.GetAllAsync(ct);
        return Results.Ok(transactions);
    }
}