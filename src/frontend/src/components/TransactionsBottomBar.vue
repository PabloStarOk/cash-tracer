<script setup lang="ts">
import { Button, Card } from 'primevue'

defineEmits<{
  add: []
}>()
</script>

<template>
  <Card>
    <template #content>
      <div class="flex justify-end">
        <Button icon="pi pi-plus" label="Add" @click="$emit('add')" />
      </div>
    </template>
  </Card>
</template>
