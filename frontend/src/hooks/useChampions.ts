import { useEffect, useState } from 'react'

import { getChampions } from '../api/championsApi'
import type { Champion } from '../types'

const CACHE_KEY = 'wildrift:champions'

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

function readCache(): Champion[] | null {
  try {
    const raw = sessionStorage.getItem(CACHE_KEY)
    return raw ? (JSON.parse(raw) as Champion[]) : null
  } catch {
    return null
  }
}

function writeCache(champions: Champion[]): void {
  try {
    sessionStorage.setItem(CACHE_KEY, JSON.stringify(champions))
  } catch {
    // sessionStorage may be unavailable (private browsing quota, etc.)
  }
}

export function useChampions() {
  const cached = readCache()
  const [champions, setChampions] = useState<Champion[]>(cached ?? fallbackChampions)
  const [usingFallback, setUsingFallback] = useState(cached === null)

  useEffect(() => {
    if (cached !== null) {
      return
    }

    const controller = new AbortController()

    async function loadChampions() {
      try {
        const response = await getChampions()
        if (controller.signal.aborted) return
        setChampions(response)
        setUsingFallback(false)
        writeCache(response)
      } catch {
        if (controller.signal.aborted) return
        setChampions(fallbackChampions)
        setUsingFallback(true)
      }
    }

    void loadChampions()

    return () => controller.abort()
  }, []) // eslint-disable-line react-hooks/exhaustive-deps

  return { champions, usingFallback }
}
