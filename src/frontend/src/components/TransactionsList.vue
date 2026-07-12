<script setup lang="ts">
import type { Transaction } from '@/types/transactions'
import { Card, Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import TransactionsTable from '@/components/TransactionsTable.vue'
import SearchBar from '@/components/SearchBar.vue'

interface Props {
  allTransactions: Transaction[]
  expenses: Transaction[]
  incomes: Transaction[]
}

defineProps<Props>()
defineEmits<{
  edit: [transaction: Transaction]
  delete: [transaction: Transaction]
}>()
const search = defineModel<string>('search', { required: true })

type TabType = 'all' | 'expenses' | 'incomes'
const activeTab: TabType = 'all'
const tabs: { type: TabType; label: string; icon: string }[] = [
  { type: 'all', label: 'All', icon: 'pi-table' },
  { type: 'expenses', label: 'Expenses', icon: 'pi-money-bill' },
  { type: 'incomes', label: 'Incomes', icon: 'pi-wallet' },
]
</script>

<template>
  <Card>
    <template #header>
      <div class="flex flex-col gap-2 px-5 pt-5 md:flex-row justify-between">
        <h2 class="text-xl font-medium">Transactions</h2>
        <SearchBar v-model="search" class="max-w-md" />
      </div>
    </template>
    <template #content>
      <Tabs :value="activeTab">
        <TabList>
          <Tab v-for="tab in tabs" :key="tab.type" :value="tab.type">
            <div class="flex gap-2 items-center">
              <i :class="`pi ${tab.icon}`" aria-hidden="true"></i>
              <p>{{ tab.label }}</p>
            </div>
          </Tab>
        </TabList>
        <TabPanels>
          <TabPanel value="all">
            <TransactionsTable
              :transactions="allTransactions"
              @edit="(transaction) => $emit('edit', transaction)"
              @delete="(transaction) => $emit('delete', transaction)"
            />
          </TabPanel>
          <TabPanel value="expenses">
            <TransactionsTable
              :transactions="expenses"
              @edit="(transaction) => $emit('edit', transaction)"
              @delete="(transaction) => $emit('delete', transaction)"
            />
          </TabPanel>
          <TabPanel value="incomes">
            <TransactionsTable
              :transactions="incomes"
              @edit="(transaction) => $emit('edit', transaction)"
              @delete="(transaction) => $emit('delete', transaction)"
            />
          </TabPanel>
        </TabPanels>
      </Tabs>
    </template>
  </Card>
</template>
