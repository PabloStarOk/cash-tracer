using CashTracer.Api.Extensions;
using CashTracer.Application.Dtos;
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
        routeBuilder.MapPost(string.Empty, HandleAsync)
            .WithSummary("Add transaction")
            .WithDescription("Adds a new financial transaction.")
            .Produces<TransactionDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }

    /// <summary>
    /// Documents the endpoint for adding a new transaction.
    /// </summary>
    /// <param name="request">The request containing the details of the transaction to add.</param>
    /// <param name="service">The service used to add the transaction.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A result indicating the outcome of the operation.</returns>
    public static async Task<IResult> HandleAsync(
        [FromBody] AddTransactionRequest request,
        [FromServices] ITransactionService service,
        CancellationToken ct)
    {
        var result = await service.AddAsync(request, ct);
        return result.ToHttpResult(r => Results.Ok(r.Value));
    }
}