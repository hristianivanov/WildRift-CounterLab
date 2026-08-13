import type { Champion } from '../types'
import { api } from './api'

export async function getChampions(signal?: AbortSignal): Promise<Champion[]> {
  const response = await api.get<Champion[]>('/champions', { signal })
  return response.data
}
