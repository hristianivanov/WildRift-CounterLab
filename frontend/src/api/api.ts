import axios from 'axios'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL?.trim() || 'http://localhost:5069/api'

export const configuredApiBaseUrl = apiBaseUrl

export const api = axios.create({
  baseURL: apiBaseUrl,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000,
})

export function getApiErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const responseData = error.response?.data
    const message =
      typeof responseData === 'object' && responseData !== null && 'error' in responseData
        ? responseData.error
        : undefined
    const details =
      typeof responseData === 'object' && responseData !== null && 'details' in responseData
        ? responseData.details
        : undefined

    if (typeof message === 'string' && message.length > 0) {
      return typeof details === 'string' && details.length > 0 ? `${message} ${details}` : message
    }

    if (error.code === 'ECONNABORTED') {
      return 'The request timed out. Try again.'
    }

    if (!error.response) {
      return 'Could not reach the API. Make sure the backend is running.'
    }
  }

  return 'Something went wrong while analyzing the draft.'
}
