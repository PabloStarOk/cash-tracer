<script setup lang="ts">
import ThemeToggle from '@/components/ThemeToggle.vue'
</script>

<template>
  <header class="flex p-6 bg-highlight">
    <div class="flex gap-4 items-center justify-between w-full">
      <h1 class="text-lg font-bold text-primary">Cash Tracer</h1>
      <ThemeToggle class="flex-1" />
    </div>
  </header>
</template>
