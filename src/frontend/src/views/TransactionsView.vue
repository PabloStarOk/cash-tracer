<script setup lang="ts">
import TransactionsBottomBar from '@/components/TransactionsBottomBar.vue'
import TransactionsFormDialog from '@/components/TransactionFormDialog.vue'
import TransactionsList from '@/components/TransactionsList.vue'
import { CURRENCIES } from '@/data/transactions'
import { useTransactionStore } from '@/stores/useTransactionsStore'
import { type Money, type Transaction, type TransactionType } from '@/types/transactions'
import { onMounted, ref } from 'vue'
import { ProgressSpinner, useConfirm, useToast } from 'primevue'
import ConfirmDialog from 'primevue/confirmdialog'
import Toast, { type ToastMessageOptions } from 'primevue/toast'
import { AxiosError } from 'axios'
import { API_ERRORS_DICTIONARY, ApiError, AXIOS_ERRORS_DICTIONARY } from '@/errors/errors'

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

function showErrorToast(error: unknown, summary: string, defaultDetail: string) {
  let detail: string | undefined
  if (error instanceof ApiError) {
    detail = API_ERRORS_DICTIONARY[error.code]
  } else if (error instanceof AxiosError && error.code) {
    detail = AXIOS_ERRORS_DICTIONARY[error.code]
  }

  toast.add({
    severity: 'error',
    summary: summary,
    detail: detail ?? defaultDetail,
    life: toastDuration,
  })
}

function buildLoadingToastOptions(summary: string, detail: string): ToastMessageOptions {
  return {
    severity: 'secondary',
    summary,
    detail,
    group: 'loading',
  }
}

async function addTransaction(
  transactionType: TransactionType,
  concept: string,
  date: Date,
  price: Money,
) {
  const loadingOptions = buildLoadingToastOptions(
    'Adding transaction',
    `Adding transaction '${concept}'`,
  )
  toast.add(loadingOptions)
  try {
    await store.add(transactionType, concept, date, price)
    toast.remove(loadingOptions)
    toast.add({
      severity: 'success',
      summary: 'Transaction added successfully',
      detail: `Transaction '${concept}' added.`,
      life: toastDuration,
    })
  } catch (error: unknown) {
    toast.remove(loadingOptions)
    showErrorToast(
      error,
      'Transaction could not be added',
      `Transaction '${concept}' could not be added due to an unexpected error, try again later.`,
    )
  }
}

async function updateTransaction(
  id: number,
  transactionType: TransactionType,
  concept: string,
  date: Date,
  price: Money,
) {
  const loadingOptions = buildLoadingToastOptions(
    'Updating transaction',
    `Updating transaction '${concept}'`,
  )
  toast.add(loadingOptions)
  try {
    await store.update(id, transactionType, concept, date, price)
    toast.remove(loadingOptions)
    toast.add({
      severity: 'success',
      summary: 'Transaction updated successfully',
      detail: `Transaction '${concept}' updated.`,
      life: toastDuration,
    })
  } catch (error: unknown) {
    toast.remove(loadingOptions)
    showErrorToast(
      error,
      'Transaction could not be updated',
      `Transaction '${concept}' could not be updated due to an unexpected error, try again later.`,
    )
  }
}

async function deleteTransaction(transaction: Transaction) {
  const loadingOptions = buildLoadingToastOptions(
    'Deleting transaction',
    `Deleting transaction '${transaction.concept}'`,
  )
  toast.add(loadingOptions)
  try {
    await store.remove(transaction.id)
    toast.remove(loadingOptions)
    toast.add({
      severity: 'success',
      summary: 'Transaction deleted successfully',
      detail: `Transaction '${transaction.concept}' deleted.`,
      life: toastDuration,
    })
  } catch (error: unknown) {
    toast.remove(loadingOptions)
    showErrorToast(
      error,
      'Transaction could not be deleted',
      `Transaction '${transaction.concept}' could not be deleted due to an unexpected error, try again later.`,
    )
  }
}

async function confirmDeleteTransaction(transaction: Transaction) {
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
    accept: async () => deleteTransaction(transaction),
  })
}

onMounted(() => {
  store.loadAll()
})
</script>

<template>
  <div class="flex gap-4 h-full">
    <div class="flex flex-col flex-1 gap-4 max-w-full">
      <TransactionsList
        v-model:search="store.search"
        v-model:date="store.date"
        :allTransactions="store.transactions"
        :expenses="store.expenses"
        :incomes="store.incomes"
        :loadingAll="store.loading"
        :loadingExpenses="store.loading"
        :loadingIncomes="store.loading"
        :errorLoadingAll="store.error"
        :errorLoadingExpenses="store.error"
        :errorLoadingIncomes="store.error"
        class="flex-1"
        @edit="openEditDialog"
        @delete="confirmDeleteTransaction"
      />
      <TransactionsBottomBar v-model:date="store.date" @add="openDialog" class="sticky bottom-0" />
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
  <Toast position="bottom-left" group="loading">
    <template #messageicon>
      <ProgressSpinner
        style="width: 1.5rem; height: 1.5rem"
        strokeWidth="8"
        animationDuration="2s"
      />
    </template>
  </Toast>
</template>
