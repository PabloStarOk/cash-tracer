export type TransactionType = 'expense' | 'income'

export interface Currency {
  code: string
  region: string
}

export interface Price {
  currency: string
  amount: number
}

export interface Transaction {
  id: number
  type: TransactionType
  concept: string
  date: Date
  price: Price
}
