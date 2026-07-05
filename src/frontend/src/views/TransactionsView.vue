<script setup lang="ts">
import TransactionsBottomBar from '@/components/TransactionsBottomBar.vue'
import TransactionsList from '@/components/TransactionsList.vue'
</script>

<template>
  <div class="flex gap-4 h-full">
    <div class="flex flex-col flex-1 gap-4 overflow-hidden">
      <TransactionsList class="flex-1" />
      <TransactionsBottomBar />
    </div>
  </div>
</template>
