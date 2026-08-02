using CashTracer.Domain.Common;

namespace CashTracer.UnitTests.Domain.Common;

public class ResultTests
{
    [Fact]
    public void Success_should_ReturnResultWithValidProperties()
    {
        // Arrange
        var expectedValue = "Sample result value.";

        // Act
        var result = Result<string>.Success(expectedValue);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedValue, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_should_ReturnResultWithValidProperties()
    {
        // Arrange
        var error = new Error(ErrorType.Validation, "Test", "Test error message.");

        // Act
        var result = Result<string>.Failure(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.Error, error);
    }

    [Fact]
    public void Failure_when_GivenErrorIsNull_should_ThrowArgumentNullException()
    {
        // Arrange
        Error? error = null;

        // Act
        Assert.Throws<ArgumentNullException>(() => Result<string>.Failure(error!));
    }

    [Fact]
    public void ValueProperty_when_ItIsTriedToBeAccessed_should_ThrowInvalidOperationException()
    {
        // Arrange
        var error = new Error(ErrorType.Validation, "Test", "Test error message.");

        // Act
        var result = Result<string>.Failure(error);

        // Assert
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }
}