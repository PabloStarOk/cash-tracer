import { useTransactionsApi } from '@/api/transactionsApi'
import type { Money, Transaction, TransactionType } from '@/types/transactions'
import { Temporal } from '@js-temporal/polyfill'
import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useTransactionStore = defineStore('transactions', () => {
  const api = useTransactionsApi()
  const transactionsMap = ref<Map<number, Transaction>>(new Map())
  const search = ref<string>('')
  const filterDate = ref<Temporal.PlainYearMonth>(Temporal.Now.plainDateISO().toPlainYearMonth())
  const loading = ref<boolean>(false)
  const error = ref<boolean>(false)
  const deletingTransactionIds = ref<Set<number>>(new Set())
  const transactions = computed(() => {
    const allTransactions = [...transactionsMap.value.values()]
    const query = search.value.trim().toLowerCase()
    return allTransactions.filter(
      (t) =>
        t.date.year === filterDate.value.year &&
        t.date.month === filterDate.value.month &&
        t.concept.toLowerCase().includes(query),
    )
  })
  const expenses = computed(() => transactions.value.filter((t) => t.type === 'expense'))
  const incomes = computed(() => transactions.value.filter((t) => t.type === 'income'))

  async function loadAll() {
    loading.value = true
    error.value = false
    try {
      const transactions = await api.getAll()
      transactions.forEach((t) => {
        transactionsMap.value.set(t.id, t)
      })
    } catch {
      error.value = true
    } finally {
      loading.value = false
    }
  }

  async function add(
    type: TransactionType,
    concept: string,
    date: Temporal.PlainDate,
    money: Money,
  ) {
    const transaction: Transaction = {
      id: 0,
      concept,
      type,
      date,
      money,
    }
    const newTransaction = await api.add(transaction)
    transactionsMap.value.set(newTransaction.id, newTransaction)
  }

  async function update(
    id: number,
    type: TransactionType,
    concept: string,
    date: Temporal.PlainDate,
    money: Money,
  ) {
    const transaction = transactionsMap.value.get(id)
    if (!transaction) return
    let updatedTransaction: Transaction = {
      id,
      concept,
      type,
      date,
      money,
    }
    updatedTransaction = await api.update(updatedTransaction)
    transactionsMap.value.set(updatedTransaction.id, updatedTransaction)
  }

  async function remove(id: number) {
    deletingTransactionIds.value.add(id)
    try {
      await api.remove(id)
      transactionsMap.value.delete(id)
    } finally {
      deletingTransactionIds.value.delete(id)
    }
  }

  return {
    search,
    filterDate,
    transactions,
    expenses,
    incomes,
    loading,
    deletingTransactionIds,
    error,
    add,
    update,
    remove,
    loadAll,
  }
})
