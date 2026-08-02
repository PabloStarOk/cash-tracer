using CashTracer.Application.Interfaces;
using CashTracer.Application.Requests;
using CashTracer.Application.Services;
using CashTracer.Domain.Common;
using CashTracer.Domain.Entities;
using CashTracer.Domain.Enums;
using CashTracer.Domain.Errors;
using CashTracer.Domain.Repositories;
using CashTracer.Domain.ValueObjects;

using Moq;

namespace CashTracer.UnitTests.Application.Services;

public class TransactionServiceTests : IDisposable
{
    private readonly Mock<ITransactionRepository> _repositoryMock;
    private readonly ITransactionService _transactionService;

    public TransactionServiceTests()
    {
        _repositoryMock = new Mock<ITransactionRepository>();
        _transactionService = new TransactionService(_repositoryMock.Object);
    }

    public void Dispose()
    {
        _repositoryMock.VerifyAll();
    }

    [Theory]
    [MemberData(nameof(GetInvalidMoneys))]
    public async Task AddAsync_when_MoneyCreationFails_should_ReturnFailure(
        string invalidCurrency,
        decimal invalidAmount,
        Error error)
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var request = new AddTransactionRequest(
            TransactionType.Income,
            "Test concept",
            new DateOnly(2026, 1, 1),
            invalidCurrency,
            invalidAmount);

        // Act
        var result = await _transactionService.AddAsync(request, ct);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Theory]
    [MemberData(nameof(GetInvalidConcepts))]
    public async Task AddAsync_when_TransactionCreationFails_should_ReturnFailure(
        string? invalidConcept,
        Error error)
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var request = new AddTransactionRequest(
            TransactionType.Income,
            invalidConcept!,
            new DateOnly(2026, 1, 1),
            "USD",
            100m);

        // Act
        var result = await _transactionService.AddAsync(request, ct);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Theory]
    [MemberData(nameof(GetValidRequests))]
    public async Task AddAsync_when_TransactionIsAddedSuccessfully_should_ReturnSuccess(AddTransactionRequest request)
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var transactionId = 1;
        var money = Money.Create(request.Currency, request.Amount).Value!;
        var transaction = Transaction
            .CreateWithId(transactionId, request.Type, request.Concept, request.Date, money)
            .Value!;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        // Act
        var result = await _transactionService.AddAsync(request, ct);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(transactionId, result.Value.Id);
        Assert.Equal(request.Type, result.Value.Type);
        Assert.Equal(request.Concept, result.Value.Concept);
        Assert.Equal(request.Date, result.Value.Date);
        Assert.Equal(request.Currency, result.Value.Money.Currency);
        Assert.Equal(request.Amount, result.Value.Money.Amount);
    }

    public static TheoryData<string, decimal, Error> GetInvalidMoneys()
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

    public static TheoryData<string?, Error> GetInvalidConcepts()
    {
        return new()
        {
            { null, TransactionErrors.NullOrEmptyConcept },
            { string.Empty, TransactionErrors.NullOrEmptyConcept },
            { "   ", TransactionErrors.NullOrEmptyConcept },
            { new string('*', Transaction.MaxConceptLength + 1), TransactionErrors.ConceptTooLong }
        };
    }

    public static TheoryData<AddTransactionRequest> GetValidRequests()
    {
        return
        [
            new AddTransactionRequest(
                TransactionType.Income,
                "Test concept",
                new DateOnly(2026, 12, 23),
                "USD",
                100m),
            new AddTransactionRequest(
                TransactionType.Expense,
                "Test concept",
                new DateOnly(2026, 5, 12),
                "COP",
                100.000m),
            new AddTransactionRequest(
                TransactionType.Expense,
                new string('*', Transaction.MaxConceptLength),
                new DateOnly(2026, 1, 13),
                "COP",
                10000m),
            new AddTransactionRequest(
                TransactionType.Income,
                "Canserbero",
                new DateOnly(1988, 3, 11),
                "VES",
                10000000m),
        ];
    }
}