<script setup lang="ts">
import TransactionsBottomBar from '@/components/TransactionsBottomBar.vue'
import TransactionsDialog from '@/components/TransactionsDialog.vue'
import TransactionsList from '@/components/TransactionsList.vue'
import { CURRENCIES } from '@/data/transactions'
import { useTransactionStore } from '@/stores/useTransactionsStore'
import type { Transaction } from '@/types/transactions'
import { ref } from 'vue'

const store = useTransactionStore()
const isDialogVisible = ref(false)
const editingTransaction = ref<Transaction | undefined>()

function openDialog() {
  isDialogVisible.value = true
}

function openEditDialog(transaction: Transaction) {
  editingTransaction.value = transaction
  openDialog()
}
</script>

<template>
  <div class="flex gap-4 h-full">
    <div class="flex flex-col flex-1 gap-4 max-w-full">
      <TransactionsList
        :allTransactions="store.transactions"
        :expenses="store.expenses"
        :incomes="store.incomes"
        class="flex-1"
        @edit="openEditDialog"
      />
      <TransactionsBottomBar @add="openDialog" class="sticky bottom-0" />
    </div>
  </div>

  <TransactionsDialog
    v-model:visible="isDialogVisible"
    v-model:editingTransaction="editingTransaction"
    :currencies="CURRENCIES"
    @add="store.add"
    @update="store.update"
  />
</template>
