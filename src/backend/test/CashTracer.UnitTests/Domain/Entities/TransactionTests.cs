using CashTracer.Domain.Common;
using CashTracer.Domain.Entities;
using CashTracer.Domain.Enums;
using CashTracer.Domain.Errors;
using CashTracer.Domain.ValueObjects;

namespace CashTracer.UnitTests.Domain.Entities;

public class TransactionTests
{
    private static readonly int StubId = 1;
    private static readonly string StubConcept = "Test concept";
    private static readonly TransactionType StubType = TransactionType.Income;
    private static readonly DateOnly StubDate = new(2023, 1, 1);
    private static readonly Money StubMoney = Money.Reconstruct("COP", 100.00m);
    private static readonly DateTimeOffset StubCreatedAt = DateTimeOffset.FromUnixTimeSeconds(1777000);
    private static readonly DateTimeOffset StubUpdatedAt = DateTimeOffset.FromUnixTimeSeconds(1997000);

    [Theory]
    [MemberData(nameof(GetValidConcepts))]
    public void Create_should_ReturnSuccessResultWithExpectedTransaction(string concept)
    {
        // Act
        var result = Transaction.Create(StubType, concept, StubDate, StubMoney);

        // Arrange
        Assert.True(result.IsSuccess);
        Assert.Equal(StubType, result.Value.Type);
        Assert.Equal(concept, result.Value.Concept);
        Assert.Equal(StubDate, result.Value.Date);
        Assert.Equal(StubMoney, result.Value.Money);
    }

    [Theory]
    [MemberData(nameof(GetValidConcepts))]
    public void Rehydrate_should_TransactionWithExpectedProperties(string concept)
    {
        // Act
        var actual = Transaction.Rehydrate(StubId, StubType, concept, StubDate, StubMoney, StubCreatedAt, StubUpdatedAt);

        // Arrange
        Assert.Equal(StubId, actual.Id);
        Assert.Equal(StubType, actual.Type);
        Assert.Equal(concept, actual.Concept);
        Assert.Equal(StubDate, actual.Date);
        Assert.Equal(StubMoney, actual.Money);
        Assert.Equal(StubCreatedAt, actual.CreatedAt);
        Assert.Equal(StubUpdatedAt, actual.UpdatedAt);
    }

    [Theory]
    [MemberData(nameof(GetInvalidConcepts))]
    public void Create_when_ConceptIsInvalid_should_ReturnFailureResult(string? invalidConcept, Error error)
    {
        // Act
        var result = Transaction.Create(StubType, invalidConcept!, StubDate, StubMoney);

        // Arrange
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Theory]
    [MemberData(nameof(GetInvalidConcepts))]
    public void Rehydrate_when_ConceptIsInvalid_should_ThrowArgumentException(string? invalidConcept, Error error)
    {
        // Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Transaction.Rehydrate(StubId, StubType, invalidConcept!, StubDate, StubMoney, StubCreatedAt, StubUpdatedAt));
        Assert.Contains(error.Message, exception.Message);
    }

    [Theory]
    [MemberData(nameof(GetValidUpdateCombinations))]
    public void Update_when_GivenValidValues_should_MutateTransactionAndReturnSuccess(
        TransactionType? newType,
        string? newConcept,
        DateOnly? newDate,
        Money? newMoney)
    {
        // Arrange
        var transaction =
            Transaction.Rehydrate(StubId, StubType, StubConcept, StubDate, StubMoney, StubCreatedAt, StubUpdatedAt);
        var expectedType = newType ?? StubType;
        var expectedConcept = newConcept ?? StubConcept;
        var expectedDate = newDate ?? StubDate;
        var expectedMoney = newMoney ?? StubMoney;

        // Act
        var result = transaction.Update(newType, newConcept, newDate, newMoney);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(StubId, transaction.Id);
        Assert.Equal(expectedType, transaction.Type);
        Assert.Equal(expectedConcept, transaction.Concept);
        Assert.Equal(expectedDate, transaction.Date);
        Assert.Equal(expectedMoney, transaction.Money);
        Assert.NotNull(transaction.UpdatedAt);
    }

    [Theory]
    [MemberData(nameof(GetInvalidUpdateConcepts))]
    public void Update_when_GivenInvalidConcept_should_ReturnFailureResultAndKeepState(
        string? invalidConcept,
        Error expectedError)
    {
        // Arrange
        var transaction =
            Transaction.Rehydrate(StubId, StubType, StubConcept, StubDate, StubMoney, StubCreatedAt, StubUpdatedAt);

        // Act
        var result = transaction.Update(newConcept: invalidConcept);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(expectedError, result.Error);
        Assert.Equal(StubType, transaction.Type);
        Assert.Equal(StubConcept, transaction.Concept);
        Assert.Equal(StubDate, transaction.Date);
        Assert.Equal(StubMoney, transaction.Money);
        Assert.Equal(StubUpdatedAt, transaction.UpdatedAt);
    }

    public static TheoryData<string> GetValidConcepts()
    {
        return
        [
            "Test concept",
            new string('*', Transaction.MaxConceptLength)
        ];
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

    public static TheoryData<TransactionType?, string?, DateOnly?, Money?> GetValidUpdateCombinations()
    {
        return new()
        {
            { TransactionType.Expense, "Groceries", new DateOnly(2023, 2, 1), Money.Reconstruct("USD", 250.00m) },
            { null, "Updated concept", null, null },
            { null, null, new DateOnly(2023, 3, 1), null },
            { null, null, null, Money.Reconstruct("EUR", 150.00m) },
            { TransactionType.Income, null, null, null }
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
}