<script setup lang="ts">
import TransactionsBottomBar from '@/components/TransactionsBottomBar.vue'
import TransactionsFormDialog from '@/components/TransactionFormDialog.vue'
import TransactionsList from '@/components/TransactionsList.vue'
import { CURRENCIES } from '@/data/transactions'
import { useTransactionStore } from '@/stores/useTransactionsStore'
import type { Transaction } from '@/types/transactions'
import { ref } from 'vue'
import { useConfirm } from 'primevue'
import ConfirmDialog from 'primevue/confirmdialog'

const store = useTransactionStore()
const isDialogVisible = ref(false)
const editingTransaction = ref<Transaction | undefined>()
const confirm = useConfirm()

function openDialog() {
  isDialogVisible.value = true
}

function openEditDialog(transaction: Transaction) {
  editingTransaction.value = transaction
  openDialog()
}

function confirmDeleteTransaction(transaction: Transaction) {
  confirm.require({
    header: 'Delete Transaction',
    message: 'Are you sure you want to delete this transaction?',
    icon: 'pi pi-exclamation-triangle',
    acceptProps: {
      label: 'Delete',
      severity: 'danger',
    },
    rejectProps: {
      label: 'Cancel',
      severity: 'secondary',
      variant: 'outlined',
    },
    accept: () => {
      store.remove(transaction.id)
    },
  })
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
        @delete="confirmDeleteTransaction"
      />
      <TransactionsBottomBar @add="openDialog" class="sticky bottom-0" />
    </div>
  </div>

  <TransactionsFormDialog
    v-model:visible="isDialogVisible"
    v-model:editingTransaction="editingTransaction"
    :currencies="CURRENCIES"
    @add="store.add"
    @update="store.update"
  />

  <ConfirmDialog class="max-w-[90dvw]" />
</template>
