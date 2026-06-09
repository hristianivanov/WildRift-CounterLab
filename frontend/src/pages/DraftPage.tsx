import { useState } from 'react'
import { AnimatePresence, motion } from 'framer-motion'
import { FlaskConical, TriangleAlert } from 'lucide-react'

import DraftForm from '../components/draft/DraftForm'
import RecommendationCard from '../components/draft/RecommendationCard'
import { useChampions } from '../hooks/useChampions'
import { useDraftAnalysis } from '../hooks/useDraftAnalysis'
import type { DraftRecommendationRequest } from '../types'

const initialDraft: DraftRecommendationRequest = {
  role: 'Baron',
  laneEnemy: '',
  enemyTeam: [],
  includeAiExplanation: false,
}

export default function DraftPage() {
  const [draft, setDraft] = useState(initialDraft)
  const { champions, usingFallback } = useChampions()
  const { analyzeDraft, error, isLoading, result } = useDraftAnalysis()

  return (
    <main className="min-h-screen px-4 py-8 md:px-8 lg:py-12">
      <div className="mx-auto max-w-7xl">
        <header className="mb-8 max-w-3xl">
          <div className="mb-4 inline-flex items-center gap-2 rounded-full border border-cyan-300/20 bg-cyan-300/10 px-3 py-1 text-xs font-semibold uppercase tracking-[0.18em] text-cyan-200">
            <FlaskConical className="size-3.5" />
            WildRiftCounterLab
          </div>
          <h1 className="text-4xl font-bold tracking-tight text-white md:text-6xl">
            Draft with data.
            <span className="block text-cyan-300">Explain with AI.</span>
          </h1>
          <p className="mt-4 max-w-2xl text-base leading-7 text-slate-400 md:text-lg">
            The recommendation engine scores every pick deterministically. AI only explains the
            final result when you ask for it.
          </p>
        </header>

        <div className="grid gap-7 lg:grid-cols-[minmax(320px,0.8fr)_minmax(0,1.4fr)]">
          <DraftForm
            champions={champions}
            value={draft}
            isLoading={isLoading}
            usingFallback={usingFallback}
            onChange={setDraft}
            onSubmit={() => void analyzeDraft(draft)}
          />

          <section>
            {error && (
              <div className="mb-5 flex items-start gap-3 rounded-2xl border border-red-300/20 bg-red-300/10 p-4 text-sm text-red-100">
                <TriangleAlert className="mt-0.5 size-4 shrink-0" />
                {error}
              </div>
            )}

            {!result && !isLoading && (
              <div className="grid min-h-72 place-items-center rounded-3xl border border-dashed border-white/10 bg-white/[0.02] p-8 text-center">
                <div>
                  <p className="text-lg font-medium text-slate-300">Recommendations appear here</p>
                  <p className="mt-2 text-sm text-slate-500">
                    Select a lane opponent and analyze the draft.
                  </p>
                </div>
              </div>
            )}

            {isLoading && (
              <div className="space-y-4">
                {[0, 1, 2].map((item) => (
                  <div
                    key={item}
                    className="h-52 animate-pulse rounded-3xl border border-white/8 bg-white/[0.04]"
                  />
                ))}
              </div>
            )}

            <AnimatePresence mode="wait">
              {result && !isLoading && (
                <motion.div
                  key={`${result.role}-${result.laneEnemy}`}
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  exit={{ opacity: 0 }}
                  className="space-y-4"
                >
                  <div className="flex items-end justify-between gap-4">
                    <div>
                      <p className="text-xs uppercase tracking-[0.18em] text-slate-500">
                        {result.role} recommendations
                      </p>
                      <h2 className="mt-1 text-2xl font-semibold text-white">
                        Into {result.laneEnemy}
                      </h2>
                    </div>
                    <p className="text-sm text-slate-500">
                      {result.recommendations.length} ranked picks
                    </p>
                  </div>

                  {result.recommendations.length === 0 && (
                    <div className="grid min-h-56 place-items-center rounded-3xl border border-dashed border-white/10 bg-white/[0.02] p-8 text-center">
                      <div>
                        <p className="text-lg font-medium text-slate-300">No recommendations found</p>
                        <p className="mt-2 text-sm text-slate-500">
                          Try another role or enemy draft.
                        </p>
                      </div>
                    </div>
                  )}

                  {result.recommendations.map((recommendation, index) => (
                    <RecommendationCard
                      key={recommendation.champion}
                      recommendation={recommendation}
                      rank={index}
                    />
                  ))}
                </motion.div>
              )}
            </AnimatePresence>
          </section>
        </div>
      </div>
    </main>
  )
}
