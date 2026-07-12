import { TRANSACTIONS } from '@/data/transactions'
import type { Price, Transaction, TransactionType } from '@/types/transactions'
import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useTransactionStore = defineStore('transactions', () => {
  const transactionsMap = ref<Map<number, Transaction>>(new Map(TRANSACTIONS.map((t) => [t.id, t])))
  const search = ref<string>('')
  const transactions = computed(() => {
    const allTransactions = [...transactionsMap.value.values()]
    const query = search.value.trim().toLowerCase()
    if (!query) return allTransactions
    return allTransactions.filter((t) => t.concept.toLowerCase().includes(query))
  })
  const expenses = computed(() => transactions.value.filter((t) => t.type === 'expense'))
  const incomes = computed(() => transactions.value.filter((t) => t.type === 'income'))

  const nextId = ref(1)

  function add(type: TransactionType, concept: string, date: Date, price: Price) {
    const transaction: Transaction = {
      id: nextId.value,
      concept,
      type,
      date,
      price,
    }
    transactionsMap.value.set(transaction.id, transaction)
    nextId.value++
  }

  function update(id: number, type: TransactionType, concept: string, date: Date, price: Price) {
    const transaction = transactionsMap.value.get(id)
    if (!transaction) return
    const updatedTransaction: Transaction = {
      id,
      concept,
      type,
      date,
      price,
    }
    transactionsMap.value.set(transaction.id, updatedTransaction)
  }

  function remove(id: number) {
    const transaction = transactionsMap.value.get(id)
    if (!transaction) return
    transactionsMap.value.delete(id)
  }

  return {
    search,
    transactions,
    expenses,
    incomes,
    add,
    update,
    remove,
  }
})
