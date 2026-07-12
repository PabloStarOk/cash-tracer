<script setup lang="ts">
import { FloatLabel, InputText, IconField, InputIcon } from 'primevue'
const search = defineModel<string>({ required: true })
</script>

<template>
  <FloatLabel variant="in">
    <IconField>
      <InputIcon>
        <i class="pi pi-search"></i>
      </InputIcon>
      <InputText v-model="search" id="transactionSearch" size="small" />
    </IconField>
    <label for="transactionSearch">Search concept</label>
  </FloatLabel>
</template>
