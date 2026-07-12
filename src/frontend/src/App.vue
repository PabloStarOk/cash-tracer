<script setup lang="ts">
import TransactionsView from './views/TransactionsView.vue'
import Header from './components/Header.vue'
</script>

<template>
  <body class="flex flex-col h-dvh">
    <Header />
    <main class="flex-1 p-4">
      <TransactionsView />
    </main>
  </body>
</template>

<style scoped></style>
