import axios from 'axios'

export const DEFAULT_API_TIMEOUT_MS = 30_000
// AI calls are allowed extra time because the backend Groq HttpClient has a 40s timeout,
// so we give 5s of headroom for network round-trip overhead.
export const AI_EXPLANATION_TIMEOUT_MS = 45_000
export const AI_RATE_LIMIT_MESSAGE =
  'AI analysis is temporarily unavailable due to free-tier limits. Engine recommendations are still available.'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL?.trim() || 'http://localhost:5069/api'

export const configuredApiBaseUrl = apiBaseUrl

export const api = axios.create({
  baseURL: apiBaseUrl,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: DEFAULT_API_TIMEOUT_MS,
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
      return 'The request timed out. Please try again.'
    }

    if (!error.response) {
      return 'Could not reach the server. Check your connection and try again.'
    }

    const status = error.response.status
    if (status === 429) {
      return 'Too many requests. Please wait a moment and try again.'
    }
    if (status >= 500) {
      return 'The server encountered an error. Please try again later.'
    }
    if (status === 400) {
      return 'Invalid request. Please check your selections.'
    }
    if (status === 404) {
      return 'The requested resource was not found.'
    }
  }

  return 'Something went wrong while analyzing the draft.'
}

export function isAiRateLimitError(error: unknown): boolean {
  if (!axios.isAxiosError(error)) {
    return false
  }

  const responseData = error.response?.data
  const responseMessage =
    typeof responseData === 'object' && responseData !== null && 'error' in responseData
      ? responseData.error
      : undefined
  const message = `${typeof responseMessage === 'string' ? responseMessage : ''} ${error.message}`

  return (
    error.response?.status === 429 ||
    message.toLowerCase().includes('rate limit') ||
    message.toLowerCase().includes('quota') ||
    message.toLowerCase().includes('resource exhausted')
  )
}
