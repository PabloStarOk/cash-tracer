import { CONCEPT_MAX_LENGTH } from '@/data/transactions'

export class ApiError extends Error {
  code: string
  constructor(code: string) {
    super()
    this.code = code
  }
}

export const AXIOS_ERRORS_DICTIONARY: Record<string, string> = {
  ERR_NETWORK: 'A network error occurred, ensure you are online.',
  ERR_INTERNET_DISCONNECTED: 'A network error occurred, ensure you are online.',
}

export const API_ERRORS_DICTIONARY: Record<string, string> = {
  'Transaction.NullOrEmptyConcept': 'Concept must not be empty.',
  'Transaction.ConceptTooLong': `Concept length must not be greater than ${CONCEPT_MAX_LENGTH}.`,
  'Money.InvalidCurrency': 'Currency must be a 3-letter ISO 4217 code.',
  'Money.InvalidAmount': 'Amount cannot be zero or negative.',
  'TransactionService.TransactionNotFound': 'The transaction could not be found in the server.',
}
