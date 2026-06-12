import { useRef, useState } from 'react'

import { getAiExplanation } from '../api/aiApi'
import { AI_RATE_LIMIT_MESSAGE, getApiErrorMessage, isAiRateLimitError } from '../api/api'
import { getDraftRecommendations } from '../api/draftApi'
import type {
  DraftRecommendationRequest,
  DraftRecommendationResponse,
} from '../types'

export function useDraftAnalysis() {
  const [result, setResult] = useState<DraftRecommendationResponse | null>(null)
  const [isLoading, setIsLoading] = useState(false)
  const [aiLoadingChampions, setAiLoadingChampions] = useState<Set<string>>(new Set())
  const [error, setError] = useState<string | null>(null)
  const analysisId = useRef(0)

  async function analyzeDraft(request: DraftRecommendationRequest) {
    const currentAnalysisId = ++analysisId.current
    const shouldGenerateAi = request.includeAiExplanation

    setIsLoading(true)
    setResult(null)
    setAiLoadingChampions(new Set())
    setError(null)

    try {
      const deterministicResult = await getDraftRecommendations({
        ...request,
        includeAiExplanation: false,
      })

      if (analysisId.current !== currentAnalysisId) {
        return
      }

      setResult(deterministicResult)
      setIsLoading(false)

      if (!shouldGenerateAi) {
        return
      }

      const recommendationsToExplain = deterministicResult.recommendations.slice(0, 1)
      setAiLoadingChampions(new Set(recommendationsToExplain.map(({ champion }) => champion)))

      for (const recommendation of recommendationsToExplain) {
        let explanation = 'AI explanation unavailable.'

        try {
          const response = await getAiExplanation({
            role: deterministicResult.role,
            laneEnemy: deterministicResult.laneEnemy,
            enemyTeam: request.enemyTeam,
            champion: recommendation.champion,
            score: recommendation.score,
            reasons: recommendation.reasons,
            plan: recommendation.plan,
          })

          explanation = response.explanation.includes('provider rate limit')
            ? AI_RATE_LIMIT_MESSAGE
            : response.explanation || explanation
        } catch (aiError) {
          if (isAiRateLimitError(aiError)) {
            explanation = AI_RATE_LIMIT_MESSAGE
          }

          // Keep the card-level fallback and continue generating the remaining explanations.
        }

        if (analysisId.current !== currentAnalysisId) {
          return
        }

        setResult((currentResult) =>
          currentResult
            ? {
                ...currentResult,
                recommendations: currentResult.recommendations.map((item) =>
                  item.champion === recommendation.champion
                    ? { ...item, aiExplanation: explanation }
                    : item,
                ),
              }
            : currentResult,
        )
        setAiLoadingChampions((currentChampions) => {
          const nextChampions = new Set(currentChampions)
          nextChampions.delete(recommendation.champion)
          return nextChampions
        })
      }
    } catch (requestError) {
      if (analysisId.current === currentAnalysisId) {
        setError(getApiErrorMessage(requestError))
      }
    } finally {
      if (analysisId.current === currentAnalysisId) {
        setIsLoading(false)
      }
    }
  }

  return { aiLoadingChampions, analyzeDraft, error, isLoading, result }
}
