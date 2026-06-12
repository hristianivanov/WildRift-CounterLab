import type {
  DraftRecommendationRequest,
  DraftRecommendationResponse,
} from '../types'
import { api, DEFAULT_API_TIMEOUT_MS } from './api'

export async function getDraftRecommendations(
  request: DraftRecommendationRequest,
): Promise<DraftRecommendationResponse> {
  const response = await api.post<DraftRecommendationResponse>(
    '/draft/recommendations',
    request,
    {
      timeout: DEFAULT_API_TIMEOUT_MS,
    },
  )

  return response.data
}
