using CashTracer.Domain.Common;
using CashTracer.Domain.Errors;
using CashTracer.Domain.ValueObjects;

namespace CashTracer.UnitTests.Domain.ValueObjects;

public class MoneyTests
{
    [Theory]
    [InlineData("USD", 100.00)]
    [InlineData("EUR", 50.50)]
    public void Create_should_ReturnSuccessResultWithExpectedMoney(string currency, decimal amount)
    {
        // Act
        var result = Money.Create(currency, amount);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(currency, result.Value.Currency);
        Assert.Equal(amount, result.Value.Amount);
    }

    [Theory]
    [MemberData(nameof(GetInvalidInput))]
    public void Create_when_CurrencyOrAmountIsInvalid_should_ReturnFailureResult(
        string currency,
        decimal amount,
        Error error)
    {
        // Act
        var result = Money.Create(currency, amount);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Theory]
    [MemberData(nameof(GetInvalidInput))]
    public void Reconstruct_when_CurrencyOrAmountIsInvalid_should_ThrowArgumentException(
        string currency,
        decimal amount,
        Error error)
    {
        // Assert
        var exception = Assert.Throws<ArgumentException>(() => Money.Reconstruct(currency, amount));
        Assert.Contains(error.Message, exception.Message);
    }

    public static TheoryData<string, decimal, Error> GetInvalidInput()
    {
        return new()
        {
            { "", 100.00m, MoneyErrors.InvalidCurrency },
            { "CO", 100.00m, MoneyErrors.InvalidCurrency },
            { "Colombian Pesos", 100.00m, MoneyErrors.InvalidCurrency },
            { "", -50.00m, MoneyErrors.InvalidCurrency },
            { "COP", -50.00m, MoneyErrors.InvalidAmount },
            { "COP", 0m, MoneyErrors.InvalidAmount },
        };
    }
}