import type { Temporal } from '@js-temporal/polyfill'

export type TransactionType = 'expense' | 'income'

export interface Currency {
  code: string
  region: string
}

export interface Money {
  currency: string
  amount: number
}

export interface Transaction {
  id: number
  type: TransactionType
  concept: string
  date: Temporal.PlainDate
  money: Money
}
