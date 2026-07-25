import type { Champion } from '../types'
import { api } from './api'

export async function getChampions(): Promise<Champion[]> {
  const response = await api.get<Champion[]>('/champions')
  return response.data
}
