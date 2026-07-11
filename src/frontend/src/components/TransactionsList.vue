<script setup lang="ts">
import type { Transaction } from '@/types/transactions'
import { Card, Column, DataTable, Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'

interface Props {
  expenses: Transaction[]
}

defineProps<Props>()

type TabType = 'all' | 'expenses' | 'incomes'
const activeTab: TabType = 'all'
const dateFormatter = new Intl.DateTimeFormat(navigator.language, {
  year: 'numeric',
  month: 'long',
  day: 'numeric',
})

const tabs: { type: TabType; label: string; icon: string }[] = [
  { type: 'all', label: 'All', icon: 'pi-table' },
  { type: 'expenses', label: 'Expenses', icon: 'pi-money-bill' },
  { type: 'incomes', label: 'Incomes', icon: 'pi-wallet' },
]
</script>

<template>
  <Card>
    <template #title>
      <h2>Transactions</h2>
    </template>
    <template #content>
      <Tabs :value="activeTab">
        <TabList>
          <Tab v-for="tab in tabs" :key="tab.type" :value="tab.type">{{ tab.label }}</Tab>
        </TabList>
        <TabPanels>
          <TabPanel value="all">
            <p>All</p>
          </TabPanel>
          <TabPanel value="expenses">
            <DataTable
              :value="expenses"
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
          </TabPanel>
          <TabPanel value="incomes">
            <p>Incomes</p>
          </TabPanel>
        </TabPanels>
      </Tabs>
    </template>
  </Card>
</template>
