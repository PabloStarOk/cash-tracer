import { Temporal } from '@js-temporal/polyfill'

export function legacyToPlainDate(date: Date): Temporal.PlainDate {
  const year = date.getFullYear()
  const month = date.getMonth() + 1
  const day = date.getDate()
  return new Temporal.PlainDate(year, month, day)
}

export function plainDateToLegacy(date: Temporal.PlainDate): Date {
  return new Date(date.year, date.month - 1, date.day)
}
