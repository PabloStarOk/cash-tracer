<script setup lang="ts">
import TransactionsView from './views/TransactionsView.vue'
</script>

<template>
  <body class="flex flex-col h-dvh">
    <header class="flex flex-0 p-6 bg-primary-950">
      <h1 class="text-lg font-bold text-primary">Cash Tracer</h1>
    </header>
    <main class="flex-1 p-4">
      <TransactionsView />
    </main>
  </body>
</template>

<style scoped></style>
