import { useState } from 'react'

const SESSION_KEY = 'admin_api_key'

export function useAdminAuth() {
  const [apiKey, setApiKeyState] = useState<string>(() => sessionStorage.getItem(SESSION_KEY) ?? '')

  function login(key: string) {
    sessionStorage.setItem(SESSION_KEY, key)
    setApiKeyState(key)
  }

  function logout() {
    sessionStorage.removeItem(SESSION_KEY)
    setApiKeyState('')
  }

  return { apiKey, isAuthenticated: apiKey.length > 0, login, logout }
}
