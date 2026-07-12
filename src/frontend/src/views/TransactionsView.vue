<script setup lang="ts">
import TransactionsBottomBar from '@/components/TransactionsBottomBar.vue'
import TransactionsFormDialog from '@/components/TransactionFormDialog.vue'
import TransactionsList from '@/components/TransactionsList.vue'
import { CURRENCIES } from '@/data/transactions'
import { useTransactionStore } from '@/stores/useTransactionsStore'
import type { Price, Transaction, TransactionType } from '@/types/transactions'
import { ref } from 'vue'
import { useConfirm, useToast } from 'primevue'
import ConfirmDialog from 'primevue/confirmdialog'
import Toast from 'primevue/toast'

const toastDuration = 4000
const store = useTransactionStore()
const isDialogVisible = ref(false)
const editingTransaction = ref<Transaction | undefined>()
const confirm = useConfirm()
const toast = useToast()

function openDialog() {
  isDialogVisible.value = true
}

function openEditDialog(transaction: Transaction) {
  editingTransaction.value = transaction
  openDialog()
}

function addTransaction(
  transactionType: TransactionType,
  concept: string,
  date: Date,
  price: Price,
) {
  try {
    store.add(transactionType, concept, date, price)
    toast.add({
      severity: 'success',
      summary: 'Transaction added successfully',
      detail: `Transaction '${concept}' added.`,
      life: toastDuration,
    })
  } catch {
    toast.add({
      severity: 'error',
      summary: 'Transaction could not be added',
      detail: `Transaction '${concept}' could not be added due to an unexpected error, try again later.`,
      life: toastDuration,
    })
  }
}

function updateTransaction(
  id: number,
  transactionType: TransactionType,
  concept: string,
  date: Date,
  price: Price,
) {
  try {
    store.update(id, transactionType, concept, date, price)
    toast.add({
      severity: 'success',
      summary: 'Transaction updated successfully',
      detail: `Transaction '${concept}' udpated.`,
      life: toastDuration,
    })
  } catch {
    toast.add({
      severity: 'error',
      summary: 'Transaction could not be updated',
      detail: `Transaction '${concept}' could not be updated due to an unexpected error, try again later.`,
      life: toastDuration,
    })
  }
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
      try {
        store.remove(transaction.id)
        toast.add({
          severity: 'success',
          summary: 'Transaction deleted successfully',
          detail: `Transaction '${transaction.concept}' deleted.`,
          life: toastDuration,
        })
      } catch {
        toast.add({
          severity: 'error',
          summary: 'Transaction could not be deleted',
          detail: `Transaction '${transaction.concept}' could not be deleted due to an unexpected error, try again later.`,
          life: toastDuration,
        })
      }
    },
  })
}
</script>

<template>
  <div class="flex gap-4 h-full">
    <div class="flex flex-col flex-1 gap-4 max-w-full">
      <TransactionsList
        v-model:search="store.search"
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
    @add="addTransaction"
    @update="updateTransaction"
  />

  <ConfirmDialog class="max-w-[90dvw]" />
  <Toast position="bottom-left" />
</template>
