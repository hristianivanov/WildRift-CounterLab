import { useEffect, useState } from 'react'

import { getChampions } from '../api/championsApi'
import type { Champion } from '../types'

const fallbackChampions: Champion[] = [
  { id: 1, name: 'Malphite', roles: ['Baron', 'Support'], tags: ['tank', 'engage'] },
  { id: 2, name: 'Garen', roles: ['Baron'], tags: ['fighter', 'safe'] },
  { id: 3, name: 'Fiora', roles: ['Baron'], tags: ['fighter', 'true-damage'] },
  { id: 4, name: 'Camille', roles: ['Baron'], tags: ['fighter', 'mobile'] },
  { id: 5, name: 'Darius', roles: ['Baron'], tags: ['fighter', 'lane-bully'] },
  { id: 6, name: 'Dr. Mundo', roles: ['Baron', 'Jungle'], tags: ['tank', 'sustain'] },
  { id: 7, name: 'Vayne', roles: ['Baron', 'Dragon'], tags: ['marksman', 'tank-shred'] },
  { id: 8, name: 'Olaf', roles: ['Baron', 'Jungle'], tags: ['fighter', 'sustain'] },
]

export function useChampions() {
  const [champions, setChampions] = useState<Champion[]>(fallbackChampions)
  const [usingFallback, setUsingFallback] = useState(false)

  useEffect(() => {
    async function loadChampions() {
      try {
        const response = await getChampions()
        setChampions(response)
        setUsingFallback(false)
      } catch {
        setChampions(fallbackChampions)
        setUsingFallback(true)
      }
    }

    void loadChampions()
  }, [])

  return { champions, usingFallback }
}
