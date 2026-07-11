<script setup lang="ts">
import TransactionsBottomBar from '@/components/TransactionsBottomBar.vue'
import TransactionsDialog from '@/components/TransactionsDialog.vue'
import TransactionsList from '@/components/TransactionsList.vue'
import { CURRENCIES } from '@/data/transactions'
import { useTransactionStore } from '@/stores/useTransactionsStore'
import { ref } from 'vue'

const store = useTransactionStore()
const isDialogVisible = ref(false)

function openDialog() {
  isDialogVisible.value = true
}
</script>

<template>
  <div class="flex gap-4 h-full">
    <div class="flex flex-col flex-1 gap-4 max-w-full">
      <TransactionsList :expenses="store.expenses" class="flex-1" />
      <TransactionsBottomBar @add="openDialog" class="sticky bottom-0" />
    </div>
  </div>

  <TransactionsDialog v-model:visible="isDialogVisible" :currencies="CURRENCIES" @add="store.add" />
</template>
