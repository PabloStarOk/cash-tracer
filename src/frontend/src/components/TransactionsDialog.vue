<script setup lang="ts">
import { useTransactionForm } from '@/composables/useTransactionsForm'
import type { Currency, Price, Transaction, TransactionType } from '@/types/transactions'
import {
  Button,
  DatePicker,
  Dialog,
  FloatLabel,
  InputNumber,
  InputText,
  Message,
  Select,
} from 'primevue'
import { computed } from 'vue'

interface Props {
  currencies: Currency[]
}

const props = defineProps<Props>()
const visible = defineModel<boolean>('visible')
const emit = defineEmits<{
  add: [transactionType: TransactionType, concept: string, date: Date, price: Price]
  update: [id: number, transactionType: TransactionType, concept: string, date: Date, price: Price]
}>()
const editingTransaction = defineModel<Transaction | undefined>('editingTransaction')
const { typeOptions, state, errors, valid, reset, validate } = useTransactionForm(
  props.currencies,
  editingTransaction,
)

const actionLabel = computed(() => (editingTransaction.value ? 'Save' : 'Add'))
const title = computed(() => (editingTransaction.value ? 'Edit Transaction' : 'Add Transaction'))

function close() {
  reset()
  visible.value = false
}

function accept() {
  validate()
  if (!valid.value) return
  const price: Price = { currency: state.currency.code, amount: state.amount as number }
  if (editingTransaction.value) {
    emit(
      'update',
      editingTransaction.value?.id,
      state.type.type,
      state.concept?.trim() as string,
      state.date,
      price,
    )
  } else emit('add', state.type.type, state.concept?.trim() as string, state.date, price)
  close()
}
</script>

<template>
  <Dialog v-model:visible="visible" modal class="max-w-[90dvw]" @after-hide="reset">
    <template #header>
      <h2 class="font-bold text-xl">{{ title }}</h2>
    </template>
    <div class="flex flex-col gap-2">
      <div class="flex flex-col md:flex-row gap-2 w-full">
        <FloatLabel variant="in" class="flex-1 md:max-w-40">
          <Select
            inputId="transactionType"
            :options="typeOptions"
            v-model="state.type"
            class="w-full"
            optionLabel="label"
            dataKey="type"
          />
          <label for="transactionType">Type</label>
        </FloatLabel>

        <div class="flex-1">
          <FloatLabel variant="in" class="w-full">
            <InputText
              id="transactionConcept"
              v-model="state.concept"
              class="w-full"
              :invalid="!errors.concept.valid"
            />
            <label for="transactionConcept">Concept</label>
          </FloatLabel>

          <div class="min-h-6">
            <Message v-if="errors.concept.msg" severity="error" variant="simple" size="small">
              {{ errors.concept.msg }}
            </Message>
          </div>
        </div>
      </div>

      <div class="flex flex-wrap gap-2 overflow-hidden">
        <FloatLabel variant="in" class="flex-1 min-w-fit">
          <DatePicker
            inputId="transactionDate"
            v-model="state.date"
            showIcon
            iconDisplay="input"
            class="w-full"
          />
          <label for="transactionDate">Date</label>
        </FloatLabel>

        <div class="flex flex-col md:flex-row flex-1 gap-2 max-w-full">
          <FloatLabel variant="in" class="w-full md:max-w-fit">
            <Select
              :options="currencies"
              inputId="transactionCurrency"
              v-model="state.currency"
              :optionLabel="(currency) => `${currency.code} - ${currency.region}`"
              dataKey="code"
              filter
              filterPlaceholder="Search for code or region"
              class="w-full"
            >
              <template #value="slotProps">
                <span v-if="slotProps.value">
                  {{ slotProps.value.code }}
                </span>
                <span v-else>{{ slotProps.placeholder }}</span>
              </template>
              <template #option="slotProps">
                <span>{{ slotProps.option.code }} - {{ slotProps.option.region }}</span>
              </template>
              <template #dropdownicon>
                <i class="pi pi-dollar"></i>
              </template>
            </Select>
            <label for="transactionCurrency">Currency</label>
          </FloatLabel>
          <div class="w-full max-w-full">
            <FloatLabel variant="in" class="w-full">
              <InputNumber
                inputId="transactionAmount"
                v-model="state.amount"
                showButtons
                buttonLayout="horizontal"
                mode="currency"
                :currency="state.currency.code"
                :invalid="!errors.amount.valid"
                inputClass="w-full"
              >
                <template #decrementicon>
                  <i class="pi pi-minus"></i>
                </template>
                <template #incrementicon>
                  <i class="pi pi-plus"></i>
                </template>
              </InputNumber>
              <label for="transactionAmount">Amount</label>
            </FloatLabel>

            <div class="min-h-6">
              <Message v-if="errors.amount.msg" severity="error" variant="simple" size="small">
                {{ errors.amount.msg }}
              </Message>
            </div>
          </div>
        </div>
      </div>
    </div>
    <template #footer>
      <Button label="Cancel" severity="secondary" @click="close" />
      <Button :label="actionLabel" @click="accept" />
    </template>
  </Dialog>
</template>
