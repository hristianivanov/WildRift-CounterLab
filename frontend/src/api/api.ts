import axios from 'axios'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL?.trim() || 'http://localhost:5069/api'

export const api = axios.create({
  baseURL: apiBaseUrl,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000,
})

export function getApiErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const message = error.response?.data?.error

    if (typeof message === 'string' && message.length > 0) {
      return message
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
