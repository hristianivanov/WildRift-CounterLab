export interface Champion {
  id: number
  name: string
  roles: string[]
  tags: string[]
}

export interface DraftRecommendationRequest {
  role: string
  laneEnemy: string
  enemyTeam: string[]
  includeAiExplanation: boolean
}

export interface ScoreBreakdown {
  laneScore: number
  teamScore: number
  roleFitScore: number
  safetyScore: number
  scalingScore: number
  damageProfileScore?: number
  utilityScore: number
  totalScore: number
}

export interface DraftRecommendation {
  champion: string
  score: number
  scoreBreakdown: ScoreBreakdown
  reasons: string[]
  plan: string
  aiExplanation: string | null
}

export interface DraftRecommendationResponse {
  role: string
  laneEnemy: string
  recommendations: DraftRecommendation[]
}

export interface ApiError {
  error: string
  details?: string | null
  traceId?: string | null
}
