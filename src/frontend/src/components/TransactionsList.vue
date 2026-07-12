<script setup lang="ts">
import type { Transaction } from '@/types/transactions'
import { Card, Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import TransactionsTable from '@/components/TransactionsTable.vue'

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
