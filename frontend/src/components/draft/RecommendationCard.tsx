import { motion } from 'framer-motion'
import { Bot, ShieldCheck, Swords } from 'lucide-react'

import type { DraftRecommendation } from '../../types'

interface RecommendationCardProps {
  recommendation: DraftRecommendation
  rank: number
}

export default function RecommendationCard({
  recommendation,
  rank,
}: RecommendationCardProps) {
  const breakdown = [
    ['Lane', recommendation.scoreBreakdown.laneScore],
    ['Team', recommendation.scoreBreakdown.teamScore],
    ['Role fit', recommendation.scoreBreakdown.roleFitScore],
    ['Safety', recommendation.scoreBreakdown.safetyScore],
    ['Scaling', recommendation.scoreBreakdown.scalingScore],
    ...(recommendation.scoreBreakdown.damageProfileScore === undefined
      ? []
      : [['Damage', recommendation.scoreBreakdown.damageProfileScore] as const]),
    ['Utility', recommendation.scoreBreakdown.utilityScore],
  ]

  return (
    <motion.article
      initial={{ opacity: 0, y: 16 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay: rank * 0.06 }}
      className="rounded-3xl border border-white/10 bg-slate-900/75 p-5 shadow-xl shadow-black/20 backdrop-blur"
    >
      <div className="flex items-start justify-between gap-4">
        <div className="flex items-center gap-3">
          <span className="grid size-10 place-items-center rounded-xl border border-cyan-300/20 bg-cyan-300/10 text-sm font-bold text-cyan-200">
            #{rank + 1}
          </span>
          <div>
            <h3 className="text-xl font-semibold text-white">{recommendation.champion}</h3>
            <p className="text-xs uppercase tracking-[0.18em] text-slate-500">
              Deterministic recommendation
            </p>
          </div>
        </div>
        <div className="text-right">
          <p className="text-3xl font-bold text-cyan-300">{recommendation.score}</p>
          <p className="text-xs text-slate-500">total score</p>
        </div>
      </div>

      <div className="mt-5 grid grid-cols-3 gap-2 sm:grid-cols-6 lg:grid-cols-7">
        {breakdown.map(([label, score]) => (
          <div key={label} className="rounded-xl border border-white/8 bg-black/20 p-2 text-center">
            <p className="text-sm font-semibold text-slate-100">{score}</p>
            <p className="text-[10px] uppercase tracking-wide text-slate-500">{label}</p>
          </div>
        ))}
      </div>

      <div className="mt-5 grid gap-4 lg:grid-cols-2">
        <div className="rounded-2xl border border-white/8 bg-white/[0.03] p-4">
          <div className="mb-3 flex items-center gap-2 text-sm font-semibold text-white">
            <ShieldCheck className="size-4 text-cyan-300" />
            Why it works
          </div>
          <ul className="space-y-2 text-sm text-slate-300">
            {recommendation.reasons.map((reason) => (
              <li key={reason} className="flex gap-2">
                <span className="mt-2 size-1.5 shrink-0 rounded-full bg-cyan-300" />
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
        <div className="mt-4 rounded-2xl border border-violet-300/15 bg-violet-300/[0.07] p-4">
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
