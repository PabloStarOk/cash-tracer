using CashTracer.Domain.Enums;
using CashTracer.Domain.ValueObjects;

namespace CashTracer.Application.Dtos;

/// <summary>
/// Represents a data transfer object for a financial transaction.
/// </summary>
/// <param name="Id">The ID of the transaction.</param>
/// <param name="Type">The <see cref="TransactionType"/> of the transaction.</param>
/// <param name="Concept">The concept or description of the transaction.</param>
/// <param name="Date">The date of the transaction.</param>
/// <param name="Money">The monetary value of the transaction.</param>
public sealed record TransactionDto(
    int Id,
    TransactionType Type,
    string Concept,
    DateOnly Date,
    Money Money);