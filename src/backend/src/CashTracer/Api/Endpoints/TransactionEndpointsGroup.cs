namespace CashTracer.Api.Endpoints;

/// <summary>
/// Represents a group of endpoints related to financial transactions.
/// </summary>
public static class TransactionEndpointGroup
{
    /// <summary>
    /// Maps the group of transaction endpoints to the specified route builder.
    /// </summary>
    /// <param name="routeBuilder">The route builder to map the endpoint group to.</param>
    public static void MapGroup(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("transactions");
        AddTransactionEndpoint.Map(group);
    }
}