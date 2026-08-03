using CashTracer.Api.Endpoints;
using CashTracer.Application.Dtos;
using CashTracer.Application.Errors;
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

public class UpdateTransactionEndpointTests : IDisposable
{
    private static readonly UpdateTransactionRequest StubRequest = new()
    {
        Concept = "Updated concept",
    };

    private readonly MockRepository _mockRepository;
    private readonly Mock<ITransactionService> _serviceMock;

    public UpdateTransactionEndpointTests()
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
        UpdateTransactionEndpoint.Map(routeBuilder);

        // Assert
        var endpoint = Assert.Single(
            routeBuilder.DataSources
                .SelectMany(dataSource => dataSource.Endpoints)
                .OfType<RouteEndpoint>());
        var httpMethods = endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods;
        Assert.Equal("{id:int}", endpoint.RoutePattern.RawText);
        Assert.NotNull(httpMethods);
        Assert.Single(httpMethods, x => x.ToString() == HttpMethods.Patch);
    }

    [Fact]
    public async Task HandleAsync_when_ServiceReturnsSuccess_should_ReturnOkResult()
    {
        // Arrange
        var id = 10;
        var ct = TestContext.Current.CancellationToken;
        var money = Money.Create("COP", 1000m).Value!;
        var dto = new TransactionDto(id, TransactionType.Expense, "Updated concept", new DateOnly(2026, 1, 1), money);
        var expectedResult = Result<TransactionDto>.Success(dto);
        _serviceMock.Setup(s => s.UpdateAsync(id, StubRequest, ct)).ReturnsAsync(expectedResult);

        // Act
        var result = await UpdateTransactionEndpoint.HandleAsync(id, StubRequest, _serviceMock.Object, ct);

        // Assert
        var okResult = Assert.IsType<Ok<TransactionDto>>(result);
        Assert.Equal(expectedResult.Value, okResult.Value);
    }

    [Theory]
    [MemberData(nameof(GetValidationErrors))]
    public async Task HandleAsync_when_ServiceReturnsValidationFailure_should_Return400ValidationProblemDetails(
        Error error)
    {
        // Arrange
        var id = 10;
        var ct = TestContext.Current.CancellationToken;
        var expectedResult = Result<TransactionDto>.Failure(error);
        _serviceMock.Setup(s => s.UpdateAsync(id, StubRequest, ct)).ReturnsAsync(expectedResult);

        // Act
        var result = await UpdateTransactionEndpoint.HandleAsync(id, StubRequest, _serviceMock.Object, ct);

        // Assert
        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        var problemDetails = Assert.IsType<HttpValidationProblemDetails>(problemResult.ProblemDetails);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.True(problemDetails.Errors.TryGetValue(error.Code, out var errorMessages));
        Assert.Single(errorMessages, error.Message);
    }

    [Fact]
    public async Task HandleAsync_when_ServiceReturnsNotFoundFailure_should_Return404ProblemDetails()
    {
        // Arrange
        var id = 10;
        var ct = TestContext.Current.CancellationToken;
        var notFoundError = TransactionServiceErrors.TransactionNotFound(id);
        var expectedResult = Result<TransactionDto>.Failure(notFoundError);
        _serviceMock.Setup(s => s.UpdateAsync(id, StubRequest, ct)).ReturnsAsync(expectedResult);

        // Act
        var result = await UpdateTransactionEndpoint.HandleAsync(id, StubRequest, _serviceMock.Object, ct);

        // Assert
        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, problemResult.ProblemDetails.Status);
        Assert.Equal(notFoundError.Message, problemResult.ProblemDetails.Detail);
    }

    public static TheoryData<Error> GetValidationErrors()
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