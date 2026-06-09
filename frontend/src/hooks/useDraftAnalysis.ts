import { useState } from 'react'

import { getApiErrorMessage } from '../api/api'
import { getDraftRecommendations } from '../api/draftApi'
import type {
  DraftRecommendationRequest,
  DraftRecommendationResponse,
} from '../types'

export function useDraftAnalysis() {
  const [result, setResult] = useState<DraftRecommendationResponse | null>(null)
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function analyzeDraft(request: DraftRecommendationRequest) {
    setIsLoading(true)
    setError(null)

    try {
      setResult(await getDraftRecommendations(request))
    } catch (requestError) {
      setError(getApiErrorMessage(requestError))
    } finally {
      setIsLoading(false)
    }
  }

  return { analyzeDraft, error, isLoading, result }
}
