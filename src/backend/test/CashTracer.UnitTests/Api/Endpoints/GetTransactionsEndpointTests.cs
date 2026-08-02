using CashTracer.Api.Endpoints;
using CashTracer.Application.Dtos;
using CashTracer.Application.Interfaces;
using CashTracer.Domain.Enums;
using CashTracer.Domain.ValueObjects;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

using Moq;

namespace CashTracer.UnitTests.Api.Endpoints;

public class GetTransactionsEndpointTests : IDisposable
{
    private readonly MockRepository _mockRepository;
    private readonly Mock<ITransactionService> _serviceMock;

    public GetTransactionsEndpointTests()
    {
        _mockRepository = new MockRepository(MockBehavior.Loose);
        _serviceMock = _mockRepository.Create<ITransactionService>();
    }

    public void Dispose()
    {
        _mockRepository.VerifyAll();
    }

    [Fact]
    public void Map_should_AddEndpointToTheGivenRoute()
    {
        // Arrange
        var app = WebApplication.CreateBuilder().Build();
        var routeBuilder = (IEndpointRouteBuilder)app;

        // Act
        GetTransactionsEndpoint.Map(routeBuilder);

        // Assert
        var endpoint = Assert.Single(
            routeBuilder.DataSources
                .SelectMany(dataSource => dataSource.Endpoints)
                .OfType<RouteEndpoint>());
        var httpMethods = endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods;
        Assert.Equal(string.Empty, endpoint.RoutePattern.RawText);
        Assert.NotNull(httpMethods);
        Assert.Single(httpMethods, x => x.ToString() == HttpMethods.Get);
    }

    [Fact]
    public async Task HandleAsync_when_ServiceReturnsData_should_ReturnOkWithTransactions()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var money = Money.Create("COP", 12000m).Value!;
        IReadOnlyList<TransactionDto> expectedTransactions =
        [
            new TransactionDto(1, TransactionType.Expense, "Market", new DateOnly(2026, 2, 1), money),
        ];
        _serviceMock.Setup(s => s.GetAllAsync(ct)).ReturnsAsync(expectedTransactions);

        // Act
        var result = await GetTransactionsEndpoint.HandleAsync(_serviceMock.Object, ct);

        // Assert
        var okResult = Assert.IsType<Ok<IReadOnlyList<TransactionDto>>>(result);
        Assert.Equal(expectedTransactions, okResult.Value);
    }
}