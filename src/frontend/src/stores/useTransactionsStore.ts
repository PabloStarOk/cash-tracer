import { useTransactionsApi } from '@/api/transactionsApi'
import type { Money, Transaction, TransactionType } from '@/types/transactions'
import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useTransactionStore = defineStore('transactions', () => {
  const api = useTransactionsApi()
  const transactionsMap = ref<Map<number, Transaction>>(new Map())
  const search = ref<string>('')
  const date = ref<Date>(new Date())
  const loading = ref<boolean>(false)
  const error = ref<boolean>(false)
  const transactions = computed(() => {
    const allTransactions = [...transactionsMap.value.values()]
    const query = search.value.trim().toLowerCase()
    return allTransactions.filter(
      (t) =>
        t.date.getFullYear() === date.value.getFullYear() &&
        t.date.getMonth() === date.value.getMonth() &&
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

  async function add(type: TransactionType, concept: string, date: Date, money: Money) {
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
    date: Date,
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
    await api.remove(id)
    transactionsMap.value.delete(id)
  }

  return {
    search,
    date,
    transactions,
    expenses,
    incomes,
    loading,
    error,
    add,
    update,
    remove,
    loadAll,
  }
})
