import { CONCEPT_MAX_LENGTH } from '@/data/transactions'
import type { Currency, Transaction, TransactionType } from '@/types/transactions'
import { computed, nextTick, reactive, watch, watchEffect, type Ref } from 'vue'

interface TypeOption {
  type: TransactionType
  label: string
}

interface TransactionsFormState {
  type: TypeOption
  concept: string
  date: Date
  currency: Currency
  amount: number
}

interface TransactionsFormErrors {
  concept: { valid: boolean; msg?: string }
  amount: { valid: boolean; msg?: string }
}

export function useTransactionForm(
  currencies: Currency[],
  editingTransaction: Ref<Transaction | undefined>,
) {
  const defaultCurrency = currencies[0] ?? { code: 'USD', region: 'United States' }
  const typeOptions: TypeOption[] = [
    { type: 'expense', label: 'Expense' },
    { type: 'income', label: 'Income' },
  ]

  const state = reactive<TransactionsFormState>({
    type: typeOptions[0] as TypeOption,
    concept: '',
    date: new Date(),
    currency: defaultCurrency,
    amount: 0,
  })

  const errors = reactive<TransactionsFormErrors>({
    concept: { valid: true },
    amount: { valid: true },
  })

  const valid = computed(() => errors.concept.valid && errors.amount.valid)
  let isResetting = false

  watch(
    () => state.concept,
    () => {
      if (!isResetting) {
        validateConcept()
      }
    },
  )

  watch(
    () => state.amount,
    () => {
      if (!isResetting) {
        validateAmount()
      }
    },
  )

  watchEffect(() => {
    if (editingTransaction.value) setInitialState(editingTransaction.value)
  })

  function validateConcept() {
    errors.concept.valid = !!state.concept && state.concept.length <= CONCEPT_MAX_LENGTH
    if (errors.concept.valid) {
      errors.concept.msg = undefined
      return
    }

    if (!state.concept) errors.concept.msg = 'Field is required'
    else errors.concept.msg = `Must be shorter than ${CONCEPT_MAX_LENGTH}`
  }

  function validateAmount() {
    errors.amount.valid = !!state.amount && state.amount > 0
    if (errors.amount.valid) {
      errors.amount.msg = undefined
      return
    }

    errors.amount.msg = `Must be greater than 0`
  }

  function validate() {
    validateConcept()
    validateAmount()
  }

  function setInitialState(transaction: Transaction) {
    const types = typeOptions.filter((t) => t.type === transaction.type)
    const matchedCurrencies = currencies.filter((c) => c.code === transaction.price.currency)
    state.type = (types.length ? types[0] : typeOptions[0]) as TypeOption
    state.concept = transaction.concept
    state.date = transaction.date
    state.currency = (matchedCurrencies.length ? matchedCurrencies[0] : defaultCurrency) as Currency
    state.amount = transaction.price.amount
  }

  async function reset() {
    isResetting = true
    editingTransaction.value = undefined
    state.type = typeOptions[0] as TypeOption
    state.concept = ''
    state.date = new Date()
    state.currency = defaultCurrency
    state.amount = 0
    await nextTick()
    errors.concept = { valid: true }
    errors.amount = { valid: true }
    isResetting = false
  }

  return {
    typeOptions,
    state,
    errors,
    valid,
    reset,
    validate,
  }
}
