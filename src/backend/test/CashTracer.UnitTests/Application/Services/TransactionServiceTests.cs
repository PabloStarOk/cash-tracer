using CashTracer.Application.Interfaces;
using CashTracer.Application.Requests;
using CashTracer.Application.Services;
using CashTracer.Application.Errors;
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
    private readonly Transaction _stubTransaction;
    private readonly Mock<ITransactionRepository> _repositoryMock;
    private readonly ITransactionService _transactionService;

    public TransactionServiceTests()
    {
        _repositoryMock = new Mock<ITransactionRepository>();
        _transactionService = new TransactionService(_repositoryMock.Object);
        _stubTransaction = CreateTransaction(
            id: 7,
            type: TransactionType.Income,
            concept: "Salary",
            date: new DateOnly(2026, 3, 15),
            currency: "USD",
            amount: 3000m);
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

    [Fact]
    public async Task GetAllAsync_when_RepositoryReturnsTransactions_should_ReturnProjectedDtos()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var transaction = CreateTransaction(
            id: 7,
            type: TransactionType.Income,
            concept: "Salary",
            date: new DateOnly(2026, 3, 15),
            currency: "USD",
            amount: 3000m);
        _repositoryMock.Setup(r => r.GetAllAsync(ct)).ReturnsAsync([transaction]);

        // Act
        var result = await _transactionService.GetAllAsync(ct);

        // Assert
        var dto = Assert.Single(result);
        Assert.Equal(transaction.Id, dto.Id);
        Assert.Equal(transaction.Type, dto.Type);
        Assert.Equal(transaction.Concept, dto.Concept);
        Assert.Equal(transaction.Date, dto.Date);
        Assert.Equal(transaction.Money, dto.Money);
    }

    [Theory]
    [MemberData(nameof(GetValidUpdateCombinations))]
    public async Task UpdateAsync_when_TransactionIsUpdatedSuccessfully_should_ReturnSuccess(
        TransactionType? newType,
        string? newConcept,
        DateOnly? newDate,
        Money? newMoney)
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var request = new UpdateTransactionRequest
        {
            Type = newType,
            Concept = newConcept,
            Date = newDate,
            Amount = newMoney?.Amount,
            Currency = newMoney?.Currency,
        };
        var expected = Transaction.CreateWithId(
            _stubTransaction.Id,
            newType ?? _stubTransaction.Type,
            newConcept ?? _stubTransaction.Concept,
            newDate ?? _stubTransaction.Date,
            newMoney ?? _stubTransaction.Money).Value!;
        _repositoryMock.Setup(r => r.GetByIdAsync(_stubTransaction.Id, ct)).ReturnsAsync(_stubTransaction);
        _repositoryMock.Setup(r => r.UpdateAsync(It.Is<Transaction>(t => t.Id == expected.Id), ct));

        // Act
        var result = await _transactionService.UpdateAsync(_stubTransaction.Id, request, ct);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected.Id, result.Value.Id);
        Assert.Equal(expected.Type, result.Value.Type);
        Assert.Equal(expected.Concept, result.Value.Concept);
        Assert.Equal(expected.Date, result.Value.Date);
        Assert.Equal(expected.Money, result.Value.Money);
    }

    [Fact]
    public async Task UpdateAsync_when_TransactionIsNotFound_should_ReturnFailure()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var request = new UpdateTransactionRequest { Concept = "Updated" };
        var id = 10;
        _repositoryMock.Setup(r => r.GetByIdAsync(id, ct)).ReturnsAsync((Transaction?)null);

        // Act
        var result = await _transactionService.UpdateAsync(id, request, ct);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(TransactionServiceErrors.TransactionNotFound(id), result.Error);
    }

    [Theory]
    [MemberData(nameof(GetInvalidMoneys))]
    public async Task UpdateAsync_when_MoneyCreationFails_should_ReturnFailure(
        string currency,
        decimal amount,
        Error error)
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var request = new UpdateTransactionRequest { Currency = currency, Amount = amount };
        _repositoryMock.Setup(r => r.GetByIdAsync(_stubTransaction.Id, ct)).ReturnsAsync(_stubTransaction);

        // Act
        var result = await _transactionService.UpdateAsync(_stubTransaction.Id, request, ct);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Theory]
    [MemberData(nameof(GetInvalidUpdateConcepts))]
    public async Task UpdateAsync_when_TransactionCreationFails_should_ReturnFailure(
        string? invalidConcept,
        Error error)
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var request = new UpdateTransactionRequest { Concept = invalidConcept };
        _repositoryMock.Setup(r => r.GetByIdAsync(_stubTransaction.Id, ct)).ReturnsAsync(_stubTransaction);

        // Act
        var result = await _transactionService.UpdateAsync(_stubTransaction.Id, request, ct);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
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

    public static TheoryData<string?, Error> GetInvalidUpdateConcepts()
    {
        return new()
        {
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

    public static TheoryData<TransactionType?, string?, DateOnly?, Money?> GetValidUpdateCombinations()
    {
        return new()
        {
            { TransactionType.Expense, "Groceries", new DateOnly(2023, 2, 1), Money.Create("USD", 250.00m).Value! },
            { null, "Updated concept", null, null },
            { null, null, new DateOnly(2023, 3, 1), null },
            { null, null, null, Money.Create("EUR", 150.00m).Value! },
            { TransactionType.Income, null, null, null }
        };
    }

    private static Transaction CreateTransaction(
        int id,
        TransactionType type,
        string concept,
        DateOnly date,
        string currency,
        decimal amount)
    {
        var money = Money.Create(currency, amount).Value!;
        return Transaction.CreateWithId(id, type, concept, date, money).Value!;
    }
}