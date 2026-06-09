import type {
  DraftRecommendationRequest,
  DraftRecommendationResponse,
} from '../types'
import { api } from './api'

export async function getDraftRecommendations(
  request: DraftRecommendationRequest,
): Promise<DraftRecommendationResponse> {
  const response = await api.post<DraftRecommendationResponse>(
    '/draft/recommendations',
    request,
  )

  return response.data
}
