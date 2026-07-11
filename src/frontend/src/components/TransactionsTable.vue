<script setup lang="ts">
import type { Transaction } from '@/types/transactions'
import { Column, DataTable } from 'primevue'

interface Props {
  transactions: Transaction[]
}

defineProps<Props>()

const dateFormatter = new Intl.DateTimeFormat(navigator.language, {
  year: 'numeric',
  month: 'long',
  day: 'numeric',
})
</script>

<template>
  <DataTable
    :value="transactions"
    tableClass="min-w-200"
    paginator
    :rows="10"
    :rowsPerPageOptions="[5, 10, 15, 20, 25, 30, 50]"
  >
    <Column field="concept" header="Concept" class="text-wrap" />
    <Column header="Date" class="min-w-fit w-[15%]">
      <template #body="slotProps">
        {{ dateFormatter.format(slotProps.data.date) }}
      </template>
    </Column>
    <Column header="Amount" class="min-w-fit w-[10%]">
      <template #body="slotProps">
        {{ slotProps.data.price.amount }}
      </template>
    </Column>
    <Column header="Currency" class="min-w-fit w-[7%]">
      <template #body="slotProps">
        {{ slotProps.data.price.currency }}
      </template>
    </Column>
    <template #empty>
      <div class="flex flex-col items-center justify-center p-12 text-neutral-500">
        <p>No transactions registered</p>
      </div>
    </template>
  </DataTable>
</template>
