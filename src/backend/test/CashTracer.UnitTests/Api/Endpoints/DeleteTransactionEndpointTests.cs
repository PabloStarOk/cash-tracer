using CashTracer.Api.Endpoints;
using CashTracer.Application.Errors;
using CashTracer.Application.Interfaces;
using CashTracer.Domain.Common;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

using Moq;

namespace CashTracer.UnitTests.Api.Endpoints;

public class DeleteTransactionEndpointTests
{
    private readonly Mock<ITransactionService> _serviceMock;

    public DeleteTransactionEndpointTests()
    {
        _serviceMock = new Mock<ITransactionService>();
    }

    [Fact]
    public async Task HandleAsync_when_TransactionReturnSuccess_should_ReturnNoContent()
    {
        // Arrange
        var id = 23;
        var ct = TestContext.Current.CancellationToken;
        _serviceMock.Setup(s => s.DeleteAsync(id, ct)).ReturnsAsync(Result.Success);

        // Act
        var result = await DeleteTransactionEndpoint.HandleAsync(id, _serviceMock.Object, ct);

        // Assert
        Assert.IsType<NoContent>(result);
    }

    [Fact]
    public async Task HandleAsync_when_TransactionReturnFailure_should_ReturnNotFound()
    {
        // Arrange
        var id = 23;
        var ct = TestContext.Current.CancellationToken;
        var error = TransactionServiceErrors.TransactionNotFound(id);
        _serviceMock.Setup(s => s.DeleteAsync(id, ct)).ReturnsAsync(Result.Failure(error));

        // Act
        var result = await DeleteTransactionEndpoint.HandleAsync(id, _serviceMock.Object, ct);

        // Assert
        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, problemResult.ProblemDetails.Status);
        Assert.Equal(error.Message, problemResult.ProblemDetails.Detail);
    }
}