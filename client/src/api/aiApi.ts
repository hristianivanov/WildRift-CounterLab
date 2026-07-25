import type { AiExplanationRequest, AiExplanationResponse } from '../types'
import { AI_EXPLANATION_TIMEOUT_MS, api } from './api'

export async function getAiExplanation(
  request: AiExplanationRequest,
): Promise<AiExplanationResponse> {
  const response = await api.post<AiExplanationResponse>('/ai/explain', request, {
    timeout: AI_EXPLANATION_TIMEOUT_MS,
  })

  return response.data
}
