using CashTracer.Api.Extensions;
using CashTracer.Application.Dtos;
using CashTracer.Application.Interfaces;
using CashTracer.Application.Requests;

using Microsoft.AspNetCore.Mvc;

namespace CashTracer.Api.Endpoints;

/// <summary>
/// Represents the endpoint for updating an existing financial transaction.
/// </summary>
public static class UpdateTransactionEndpoint
{
    /// <summary>
    /// Maps the endpoint for updating an existing transaction to the specified route builder.
    /// </summary>
    /// <param name="routeBuilder">The route builder to map the endpoint to.</param>
    public static void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPatch("{id:int}", HandleAsync)
            .WithSummary("Update transaction")
            .WithDescription("Updates an existing financial transaction.")
            .Produces<TransactionDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Updates an existing transaction.
    /// </summary>
    /// <param name="id">The transaction identifier.</param>
    /// <param name="request">The request containing optional fields to update.</param>
    /// <param name="service">The transaction service.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A result indicating the update outcome.</returns>
    public static async Task<IResult> HandleAsync(
        int id,
        [FromBody] UpdateTransactionRequest request,
        [FromServices] ITransactionService service,
        CancellationToken ct)
    {
        var result = await service.UpdateAsync(id, request, ct);
        return result.ToHttpResult(r => Results.Ok(r.Value));
    }
}