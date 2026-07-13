<script setup lang="ts">
import { Button, Card, DatePicker, FloatLabel } from 'primevue'

defineEmits<{
  add: []
}>()

const date = defineModel<Date>('date', { required: true })
</script>

<template>
  <Card>
    <template #content>
      <div class="flex gap-4 justify-between">
        <FloatLabel variant="in" class="max-[375px]:max-w-40">
          <DatePicker
            v-model="date"
            inputId="transactionsDate"
            dateFormat="MM yy"
            view="month"
            size="small"
            showIcon
            iconDisplay="input"
            showButtonBar
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
