using CashTracer.Api.Extensions;
using CashTracer.Application.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace CashTracer.Api.Endpoints;

/// <summary>
/// Represents the endpoint for deleting a financial transaction.
/// </summary>
public static class DeleteTransactionEndpoint
{
    /// <summary>
    /// Maps the endpoint for deleting a transaction to the specified route builder.
    /// </summary>
    /// <param name="routeBuilder">The route builder to map the endpoint to.</param>
    public static void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapDelete("{id}", HandleAsync)
            .WithSummary("Delete transaction")
            .WithDescription("Deletes an existing financial transaction.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Handles the deletion of a transaction based on the provided identifier.
    /// </summary>
    /// <param name="id">The transaction identifier.</param>
    /// <param name="service">The transaction service.</param>
    /// <param name="ct">A token to monitor the request cancellation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task<IResult> HandleAsync(
        [FromRoute] int id,
        [FromServices] ITransactionService service,
        CancellationToken ct)
    {
        var result = await service.DeleteAsync(id, ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }
}