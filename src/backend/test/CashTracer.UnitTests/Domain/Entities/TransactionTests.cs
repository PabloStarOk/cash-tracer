using CashTracer.Domain.Common;
using CashTracer.Domain.Entities;
using CashTracer.Domain.Enums;
using CashTracer.Domain.Errors;
using CashTracer.Domain.ValueObjects;

namespace CashTracer.UnitTests.Domain.Entities;

public class TransactionTests
{
    private static readonly int Id = 1;
    private static readonly TransactionType Type = TransactionType.Income;
    private static readonly DateOnly Date = new(2023, 1, 1);
    private static readonly Money Money = Money.Create("COP", 100.00m).Value!;

    [Theory]
    [MemberData(nameof(GetValidConcepts))]
    public void Create_should_ReturnSuccessResultWithExpectedTransaction(string concept)
    {
        // Act
        var result = Transaction.Create(Type, concept, Date, Money);

        // Arrange
        Assert.True(result.IsSuccess);
        Assert.Equal(Type, result.Value.Type);
        Assert.Equal(concept, result.Value.Concept);
        Assert.Equal(Date, result.Value.Date);
        Assert.Equal(Money, result.Value.Money);
    }

    [Theory]
    [MemberData(nameof(GetValidConcepts))]
    public void CreateWithId_should_ReturnSuccessResultWithExpectedTransaction(string concept)
    {
        // Act
        var result = Transaction.CreateWithId(Id, Type, concept, Date, Money);

        // Arrange
        Assert.True(result.IsSuccess);
        Assert.Equal(Id, result.Value.Id);
        Assert.Equal(Type, result.Value.Type);
        Assert.Equal(concept, result.Value.Concept);
        Assert.Equal(Date, result.Value.Date);
        Assert.Equal(Money, result.Value.Money);
    }

    [Theory]
    [MemberData(nameof(GetInvalidConcepts))]
    public void Create_when_ConceptIsInvalid_should_ReturnFailureResult(string? invalidConcept, Error error)
    {
        // Act
        var result = Transaction.Create(Type, invalidConcept!, Date, Money);

        // Arrange
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Theory]
    [MemberData(nameof(GetInvalidConcepts))]
    public void CreateWithId_when_ConceptIsInvalid_should_ReturnFailureResult(string? invalidConcept, Error error)
    {
        // Act
        var result = Transaction.CreateWithId(Id, Type, invalidConcept!, Date, Money);

        // Arrange
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
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
}