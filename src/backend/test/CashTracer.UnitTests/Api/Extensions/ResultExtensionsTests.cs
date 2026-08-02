using CashTracer.Api.Extensions;
using CashTracer.Domain.Common;
using CashTracer.Domain.Errors;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CashTracer.UnitTests.Api.Extensions;

public class ResultExtensionsTests
{
    [Fact]
    public void ToHttpResult_when_ResultIsSuccess_should_ReturnSuccessHttpResult()
    {
        // Arrange
        var msg = "created";
        var result = Result<string>.Success("ok");

        // Act
        var httpResult = result.ToHttpResult(_ => Results.Ok(msg));

        // Assert
        var okResult = Assert.IsType<Ok<string>>(httpResult);
        Assert.Equal(msg, okResult.Value);
    }

    [Fact]
    public void ToHttpResult_when_ResultIsValidationFailure_should_Return400ValidationProblemDetails()
    {
        // Arrange
        var result = Result<string>.Failure(TransactionErrors.ConceptTooLong);

        // Act
        var httpResult = result.ToHttpResult(_ => Results.Ok("created"));

        // Assert
        var problemResult = Assert.IsType<ProblemHttpResult>(httpResult);
        var problemDetails = Assert.IsType<HttpValidationProblemDetails>(problemResult.ProblemDetails);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.True(problemDetails.Errors.TryGetValue(TransactionErrors.ConceptTooLong.Code, out var errorMessages));
        Assert.Single(errorMessages, TransactionErrors.ConceptTooLong.Message);
    }
}