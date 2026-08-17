using CashTracer.Api.Endpoints;
using CashTracer.Application.Dtos;
using CashTracer.Application.Interfaces;
using CashTracer.Application.Requests;
using CashTracer.Domain.Common;
using CashTracer.Domain.Enums;
using CashTracer.Domain.Errors;
using CashTracer.Domain.ValueObjects;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

using Moq;

namespace CashTracer.UnitTests.Api.Endpoints;

public class AddTransactionEndpointTests : IDisposable
{
    private static readonly AddTransactionRequest StubRequest = new(
        Type: TransactionType.Expense,
        Concept: "Test",
        Date: new DateOnly(2026, 1, 1),
        Currency: "COP",
        Amount: 1000m);
    private readonly MockRepository _mockRepository;
    private readonly Mock<ITransactionService> _serviceMock;

    public AddTransactionEndpointTests()
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
        AddTransactionEndpoint.Map(routeBuilder);

        // Assert
        var endpoint = Assert.Single(
            routeBuilder.DataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .OfType<RouteEndpoint>());
        var httpMethods = endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods;
        Assert.Equal(string.Empty, endpoint.RoutePattern.RawText);
        Assert.NotNull(httpMethods);
        Assert.Single(httpMethods, x => x.ToString() == HttpMethods.Post);
    }

    [Fact]
    public async Task HandleAsync_when_ServiceReturnsSuccess_should_ReturnOkResult()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var money = Money.Create(StubRequest.Currency, StubRequest.Amount).Value!;
        var dto = new TransactionDto(1, StubRequest.Type, StubRequest.Concept, StubRequest.Date, money);
        var expectedResult = Result<TransactionDto>.Success(dto);
        _serviceMock.Setup(s => s.AddAsync(StubRequest, ct)).ReturnsAsync(expectedResult);

        // Act
        var result = await AddTransactionEndpoint.HandleAsync(StubRequest, _serviceMock.Object, ct);

        // Assert
        var okResult = Assert.IsType<Ok<TransactionDto>>(result);
        Assert.Equal(expectedResult.Value, okResult.Value);
    }

    [Theory]
    [MemberData(nameof(GetErrors))]
    public async Task HandleAsync_when_ServiceReturnsFailure_should_Return400ValidationProblemDetails(Error error)
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var expectedResult = Result<TransactionDto>.Failure(error);
        _serviceMock.Setup(s => s.AddAsync(StubRequest, ct)).ReturnsAsync(expectedResult);

        // Act
        var result = await AddTransactionEndpoint.HandleAsync(StubRequest, _serviceMock.Object, ct);

        // Assert
        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        var problemDetails = Assert.IsType<HttpValidationProblemDetails>(problemResult.ProblemDetails);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.True(problemDetails.Errors.TryGetValue(error.Code, out var errorMessages));
        Assert.Single(errorMessages, error.Message);
    }

    public static TheoryData<Error> GetErrors()
    {
        return
        [
            TransactionErrors.ConceptTooLong,
            TransactionErrors.NullOrEmptyConcept,
            MoneyErrors.InvalidCurrency,
            MoneyErrors.InvalidAmount,
        ];
    }
}