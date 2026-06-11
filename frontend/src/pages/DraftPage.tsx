import { useState } from 'react'
import { AnimatePresence, motion } from 'framer-motion'
import { BrainCircuit, DatabaseZap, FlaskConical, ShieldCheck } from 'lucide-react'

import EmptyState from '../components/common/EmptyState'
import ErrorState from '../components/common/ErrorState'
import LoadingState from '../components/common/LoadingState'
import DraftForm from '../components/draft/DraftForm'
import RecommendationCard from '../components/draft/RecommendationCard'
import PageShell from '../components/layout/PageShell'
import { useChampions } from '../hooks/useChampions'
import { useDraftAnalysis } from '../hooks/useDraftAnalysis'
import type { DraftRecommendationRequest } from '../types'

const initialDraft: DraftRecommendationRequest = {
  role: 'Baron',
  laneEnemy: '',
  enemyTeam: [],
  includeAiExplanation: false,
}

const productSignals = [
  { icon: DatabaseZap, label: 'Rule-based scoring' },
  { icon: ShieldCheck, label: 'Role-aware picks' },
  { icon: BrainCircuit, label: 'Optional AI context' },
]

export default function DraftPage() {
  const [draft, setDraft] = useState(initialDraft)
  const { champions, usingFallback } = useChampions()
  const { analyzeDraft, error, isLoading, result } = useDraftAnalysis()

  return (
    <PageShell>
      <header className="mb-6 overflow-hidden rounded-[24px] border border-white/8 bg-gradient-to-br from-slate-900/75 via-slate-900/35 to-cyan-950/20 px-4 py-7 shadow-2xl shadow-black/20 sm:mb-8 sm:rounded-[32px] sm:px-8 sm:py-8 lg:px-10 lg:py-10">
        <div className="max-w-4xl">
          <div className="mb-5 inline-flex items-center gap-2 rounded-full border border-cyan-300/20 bg-cyan-300/10 px-3 py-1.5 text-[10px] font-bold uppercase tracking-[0.2em] text-cyan-200">
            <FlaskConical className="size-3.5" />
            Deterministic draft intelligence
          </div>
          <h1 className="max-w-3xl text-3xl font-black tracking-[-0.04em] text-white min-[390px]:text-4xl sm:text-5xl lg:text-6xl xl:text-7xl">
            Find the right answer
            <span className="block bg-gradient-to-r from-cyan-300 via-sky-300 to-violet-300 bg-clip-text text-transparent">
              before draft locks.
            </span>
          </h1>
          <p className="mt-5 max-w-2xl text-sm leading-7 text-slate-400 sm:text-base lg:text-lg">
            Compare matchup strength, team fit, safety, scaling, and utility in seconds. The engine
            ranks every pick from rules and data; AI only explains the result.
          </p>

          <div className="mt-7 flex flex-wrap gap-2">
            {productSignals.map(({ icon: Icon, label }) => (
              <div
                key={label}
                className="inline-flex items-center gap-2 rounded-xl border border-white/8 bg-black/15 px-3 py-2 text-xs font-medium text-slate-300"
              >
                <Icon className="size-3.5 text-cyan-300" />
                {label}
              </div>
            ))}
          </div>
        </div>
      </header>

      <div className="grid min-w-0 items-start gap-6 lg:grid-cols-[minmax(310px,0.72fr)_minmax(0,1.35fr)] xl:grid-cols-[minmax(350px,0.72fr)_minmax(0,1.35fr)] xl:gap-8">
        <DraftForm
          champions={champions}
          value={draft}
          isLoading={isLoading}
          usingFallback={usingFallback}
          onChange={setDraft}
          onSubmit={() => void analyzeDraft(draft)}
        />

        <section className="min-w-0">
          {error && <ErrorState message={error} />}
          {!result && !isLoading && <EmptyState />}
          {isLoading && <LoadingState />}

          <AnimatePresence mode="wait">
            {result && !isLoading && (
              <motion.div
                key={`${result.role}-${result.laneEnemy}`}
                initial={{ opacity: 0, y: 8 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0 }}
                className="space-y-4"
              >
                <div className="mb-5 flex flex-wrap items-end justify-between gap-3">
                  <div>
                    <p className="text-[10px] font-bold uppercase tracking-[0.2em] text-cyan-300/70">
                      Analysis complete
                    </p>
                    <h2 className="mt-1 text-2xl font-bold tracking-tight text-white sm:text-3xl">
                      {result.role} picks into {result.laneEnemy}
                    </h2>
                  </div>
                  <p className="rounded-full border border-white/8 bg-white/[0.03] px-3 py-1.5 text-xs font-medium text-slate-400">
                    {result.recommendations.length} ranked picks
                  </p>
                </div>

                {result.recommendations.length === 0 && (
                  <EmptyState
                    title="No recommendations found"
                    message="Try another role or adjust the enemy draft."
                  />
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
    </PageShell>
  )
}
