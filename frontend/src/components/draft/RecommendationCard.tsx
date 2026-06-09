import { motion } from 'framer-motion'
import { Bot, ChevronRight, ShieldCheck, Swords } from 'lucide-react'

import type { DraftRecommendation } from '../../types'
import ScoreBadge from './ScoreBadge'
import ScoreBreakdown from './ScoreBreakdown'

interface RecommendationCardProps {
  recommendation: DraftRecommendation
  rank: number
}

export default function RecommendationCard({
  recommendation,
  rank,
}: RecommendationCardProps) {
  return (
    <motion.article
      initial={{ opacity: 0, y: 16 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay: rank * 0.06 }}
      className={`group relative overflow-hidden rounded-[28px] border bg-slate-900/75 p-5 shadow-xl shadow-black/20 backdrop-blur transition hover:-translate-y-0.5 hover:border-cyan-300/20 hover:shadow-cyan-950/20 sm:p-6 ${
        rank === 0 ? 'border-cyan-300/25' : 'border-white/10'
      }`}
    >
      {rank === 0 && (
        <div className="absolute inset-x-0 top-0 h-px bg-gradient-to-r from-transparent via-cyan-300/70 to-transparent" />
      )}
      <div className="flex items-start justify-between gap-4">
        <div className="flex items-center gap-3">
          <span className="grid size-11 place-items-center rounded-2xl border border-cyan-300/20 bg-cyan-300/10 text-sm font-black text-cyan-200">
            #{rank + 1}
          </span>
          <div>
            <p className="text-[10px] font-semibold uppercase tracking-[0.18em] text-cyan-300/70">
              {rank === 0 ? 'Best fit' : 'Recommended pick'}
            </p>
            <h3 className="mt-1 text-2xl font-bold tracking-tight text-white">
              {recommendation.champion}
            </h3>
          </div>
        </div>
        <ScoreBadge score={recommendation.score} />
      </div>

      <div className="mt-6">
        <p className="mb-3 text-[10px] font-semibold uppercase tracking-[0.16em] text-slate-500">
          Score breakdown
        </p>
        <ScoreBreakdown breakdown={recommendation.scoreBreakdown} />
      </div>

      <div className="mt-6 grid gap-3 lg:grid-cols-2">
        <div className="rounded-2xl border border-white/8 bg-white/[0.03] p-4">
          <div className="mb-3 flex items-center gap-2 text-sm font-semibold text-white">
            <ShieldCheck className="size-4 text-cyan-300" />
            Why it works
          </div>
          <ul className="space-y-2 text-sm text-slate-300">
            {recommendation.reasons.map((reason) => (
              <li key={reason} className="flex gap-2">
                <ChevronRight className="mt-0.5 size-4 shrink-0 text-cyan-300" />
                {reason}
              </li>
            ))}
          </ul>
        </div>

        <div className="rounded-2xl border border-white/8 bg-white/[0.03] p-4">
          <div className="mb-3 flex items-center gap-2 text-sm font-semibold text-white">
            <Swords className="size-4 text-amber-300" />
            Game plan
          </div>
          <p className="text-sm leading-6 text-slate-300">{recommendation.plan}</p>
        </div>
      </div>

      {recommendation.aiExplanation && (
        <div className="mt-4 rounded-2xl border border-violet-300/15 bg-gradient-to-br from-violet-300/[0.09] to-cyan-300/[0.04] p-4">
          <div className="mb-2 flex items-center gap-2 text-sm font-semibold text-violet-100">
            <Bot className="size-4" />
            AI explanation
          </div>
          <p className="whitespace-pre-line text-sm leading-6 text-violet-100/75">
            {recommendation.aiExplanation}
          </p>
        </div>
      )}
    </motion.article>
  )
}
