<script setup lang="ts">
import type { Transaction, TransactionType } from '@/types/transactions'
import { Column, DataTable, Tag, Button } from 'primevue'

interface Props {
  transactions: Transaction[]
}

defineProps<Props>()
defineEmits<{ edit: [transaction: Transaction] }>()

const dateFormatter = new Intl.DateTimeFormat(navigator.language, {
  year: 'numeric',
  month: 'long',
  day: 'numeric',
})

function capitalizeType(type: TransactionType): string {
  switch (type) {
    case 'income':
      return 'Income'
    case 'expense':
      return 'Expense'
  }
}
</script>

<template>
  <DataTable
    :value="transactions"
    tableClass="min-w-200"
    paginator
    :rows="10"
    :rowsPerPageOptions="[5, 10, 15, 20, 25, 30, 50]"
  >
    <Column field="concept" header="Concept" class="text-wrap">
      <template #body="slotProps">
        <div>
          <p>
            {{ slotProps.data.concept }}
          </p>
          <Tag
            :value="capitalizeType(slotProps.data.type)"
            :severity="slotProps.data.type === 'income' ? 'success' : 'warn'"
          />
        </div>
      </template>
    </Column>
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
    <Column header="Actions" class="min-w-fit w-[7%]">
      <template #body="slotProps">
        <div class="flex gap-2">
          <Button
            icon="pi pi-pencil"
            severity="secondary"
            variant="outlined"
            size="small"
            aria-label="Edit transaction"
            @click="$emit('edit', slotProps.data)"
          />
        </div>
      </template>
    </Column>
    <template #empty>
      <div class="flex flex-col items-center justify-center p-12 text-neutral-500">
        <p>No transactions registered</p>
      </div>
    </template>
  </DataTable>
</template>
