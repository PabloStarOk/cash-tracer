import { ApiError } from '@/errors/errors'
import { type Money, type Transaction, type TransactionType } from '@/types/transactions'
import axios, { AxiosError } from 'axios'

interface ProblemDetails {
  type?: string
  title: string
  status: number
  detail?: string
  instance?: string
  errors?: Record<string, string[]>
}

interface TransactionRequestDto {
  type: TransactionType
  concept: string
  date: string
  currency: string
  amount: number
}

interface TransactionResponseDto {
  id: number
  type: TransactionType
  concept: string
  date: string
  money: Money
}

export function useTransactionsApi() {
  const client = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
    timeout: import.meta.env.VITE_API_REQUEST_TIMEOUT_MS,
  })
  const transactionsPath = `/transactions`

  client.interceptors.response.use(null, function (error: AxiosError) {
    const problemDetails = error.response?.data as ProblemDetails
    if (problemDetails && problemDetails.errors) {
      const apiErrors = mapValidationErrors(problemDetails.errors)
      return Promise.reject(apiErrors[0])
    }
    return Promise.reject(error)
  })

  function mapValidationErrors(errors: Record<string, string[]>): ApiError[] {
    const errorsMap = new Map<string, string[]>(Object.entries(errors))
    const apiErrors: ApiError[] = []
    errorsMap.forEach((_, key) => {
      apiErrors.push(new ApiError(key))
    })
    return apiErrors
  }

  function toPlainDateString(date: Date): string {
    const year = String(date.getFullYear()).padStart(4, '0')
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const day = String(date.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  }

  function fromPlainDateString(date: string): Date {
    const [year, month, day] = date.split('-').map(Number)
    return new Date(year as number, (month as number) - 1, day)
  }

  function convertToDto(transaction: Transaction): TransactionRequestDto {
    return {
      type: transaction.type,
      concept: transaction.concept,
      date: toPlainDateString(transaction.date),
      currency: transaction.money.currency,
      amount: transaction.money.amount,
    }
  }

  function convertFromDto(dto: TransactionResponseDto): Transaction {
    return {
      id: dto.id,
      type: dto.type,
      concept: dto.concept,
      date: fromPlainDateString(dto.date),
      money: dto.money,
    }
  }

  async function getAll(): Promise<Transaction[]> {
    const response = await client.get<TransactionResponseDto[]>(transactionsPath)
    return response.data.map(convertFromDto)
  }

  async function add(transaction: Transaction): Promise<Transaction> {
    const requestDto = convertToDto(transaction)
    const response = await client.post<TransactionResponseDto>(transactionsPath, requestDto)
    return convertFromDto(response.data)
  }

  async function update(transaction: Transaction): Promise<Transaction> {
    const requestDto = convertToDto(transaction)
    const url = `${transactionsPath}/${transaction.id}`
    const response = await client.patch<TransactionResponseDto>(url, requestDto)
    return convertFromDto(response.data)
  }

  async function remove(transactionId: number) {
    const url = `${transactionsPath}/${transactionId}`
    await client.delete(url)
  }

  return {
    getAll,
    add,
    update,
    remove,
  }
}
