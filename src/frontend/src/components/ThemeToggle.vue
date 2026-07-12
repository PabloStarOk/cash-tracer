<script setup lang="ts">
import { useThemeManager, type SchemePreference } from '@/composables/useThemeManager'
import { Select } from 'primevue'
import { ref, watchEffect } from 'vue'

const { scheme, change } = useThemeManager()

const selectOptions: { label: string; scheme: SchemePreference; icon: string }[] = [
  {
    label: 'System',
    scheme: 'system',
    icon: 'pi-desktop',
  },
  {
    label: 'Dark',
    scheme: 'dark',
    icon: 'pi-moon',
  },
  {
    label: 'Light',
    scheme: 'light',
    icon: 'pi-sun',
  },
]
const initialOption = selectOptions.filter((o) => o.scheme === scheme.value)[0]
const selectedOption = ref(initialOption)
watchEffect(() => change(selectedOption.value?.scheme))
</script>

<template>
  <Select v-model="selectedOption" :options="selectOptions" optionLabel="label" class="max-w-42">
    <template #value="slotProps">
      <div class="flex gap-2 items-center">
        <i :class="`pi ${selectedOption?.icon}`"></i>
        <p>{{ slotProps.value.label }}</p>
      </div>
    </template>
    <template #option="slotProps">
      <div class="flex gap-2 items-center">
        <i :class="`pi ${slotProps.option.icon}`"></i>
        <p>{{ slotProps.option.label }}</p>
      </div>
    </template>
  </Select>
</template>
