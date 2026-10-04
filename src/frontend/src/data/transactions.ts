import type { Currency } from '@/types/transactions'

export const CONCEPT_MAX_LENGTH = 50

export const CURRENCIES: Currency[] = [
  { code: 'USD', region: 'United States' },
  { code: 'EUR', region: 'European Union' },
  { code: 'COP', region: 'Colombia' },
  { code: 'GBP', region: 'United Kingdom' },
  { code: 'JPY', region: 'Japan' },
  { code: 'CAD', region: 'Canada' },
  { code: 'AUD', region: 'Australia' },
  { code: 'BRL', region: 'Brazil' },
  { code: 'MXN', region: 'Mexico' },
  { code: 'CLP', region: 'Chile' },
  { code: 'PEN', region: 'Peru' },
  { code: 'ARS', region: 'Argentina' },
  { code: 'CHF', region: 'Switzerland' },
  { code: 'CNY', region: 'China' },
]
