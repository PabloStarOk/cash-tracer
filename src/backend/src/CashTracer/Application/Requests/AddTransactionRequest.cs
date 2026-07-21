using CashTracer.Domain.Enums;
using CashTracer.Domain.ValueObjects;

namespace CashTracer.Application.Requests;

/// <summary>
/// Represents a request to add a new financial transaction.
/// </summary>
/// <param name="Type">The <see cref="TransactionType"/> of the transaction.</param>
/// <param name="Concept">The concept or description of the transaction.</param>
/// <param name="Date">The date of the transaction.</param>
/// <param name="Money">The monetary value of the transaction.</param>
public sealed record AddTransactionRequest(TransactionType Type, string Concept, DateOnly Date, Money Money);