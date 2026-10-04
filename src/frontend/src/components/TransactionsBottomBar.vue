<script setup lang="ts">
import { Temporal } from '@js-temporal/polyfill'
import { Button, Card, DatePicker, FloatLabel } from 'primevue'
import { computed } from 'vue'

defineEmits<{
  add: []
}>()

const date = defineModel<Temporal.PlainYearMonth>('date', { required: true })
const legacyDate = computed(() => {
  return new Date(date.value.toPlainDate({ day: 1 }).toLocaleString())
})

function updateDate(input: unknown) {
  if (!(input instanceof Date)) return
  const value = new Temporal.PlainYearMonth(input.getFullYear(), input.getMonth() + 1)
  date.value = value
}
</script>

<template>
  <Card>
    <template #content>
      <div class="flex gap-4 justify-between">
        <FloatLabel variant="in" class="max-[375px]:max-w-40">
          <DatePicker
            :model-value="legacyDate"
            inputId="transactionsDate"
            dateFormat="MM yy"
            view="month"
            size="small"
            showIcon
            iconDisplay="input"
            showButtonBar
            @update:model-value="updateDate"
          >
            <template #buttonbar="{ todayCallback }">
              <div class="flex items-center justify-start gap-2">
                <Button label="Today" @click="todayCallback" severity="secondary" size="small" />
              </div>
            </template>
          </DatePicker>
          <label for="transactionsDate">Date</label>
        </FloatLabel>
        <Button icon="pi pi-plus" label="Add" @click="$emit('add')" />
      </div>
    </template>
  </Card>
</template>
