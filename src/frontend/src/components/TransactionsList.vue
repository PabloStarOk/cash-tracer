<script setup lang="ts">
import { Card, Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'

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
            <p>All</p>
          </TabPanel>
          <TabPanel value="expenses">
            <p>Expenses</p>
          </TabPanel>
          <TabPanel value="incomes">
            <p>Incomes</p>
          </TabPanel>
        </TabPanels>
      </Tabs>
    </template>
  </Card>
</template>
