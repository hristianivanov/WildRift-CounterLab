import { motion } from 'framer-motion'
import { Bot, ChevronRight, Crown, ShieldCheck, Sparkles, Swords } from 'lucide-react'

import type { DraftRecommendation } from '../../types'
import ChampionPortrait from '../common/ChampionPortrait'
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
      aria-label={`${rank === 0 ? 'Best pick' : `Recommendation ${rank + 1}`}: ${recommendation.champion}`}
      initial={{ opacity: 0, y: 16 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay: rank * 0.06 }}
      className={`group relative overflow-hidden rounded-[24px] border bg-slate-900/75 p-4 shadow-xl shadow-black/20 backdrop-blur transition hover:-translate-y-0.5 sm:rounded-[28px] sm:p-6 ${
        rank === 0
          ? 'border-amber-300/35 shadow-amber-950/25 hover:border-amber-300/50 hover:shadow-amber-950/35 sm:p-7'
          : 'border-white/10 hover:border-cyan-300/20 hover:shadow-cyan-950/20'
      }`}
    >
      {rank === 0 && (
        <>
          <div className="absolute inset-x-0 top-0 h-px bg-gradient-to-r from-transparent via-amber-300 to-transparent" />
          <div className="pointer-events-none absolute -top-24 right-8 size-48 rounded-full bg-amber-300/[0.08] blur-3xl" />
        </>
      )}
      <div className="relative flex flex-wrap items-start justify-between gap-4">
        <div className="flex min-w-0 items-center gap-3 sm:gap-4">
          <span
            className={`grid size-10 shrink-0 place-items-center rounded-xl border text-sm font-black sm:size-11 sm:rounded-2xl ${
              rank === 0
                ? 'border-amber-300/30 bg-amber-300/10 text-amber-200'
                : 'border-cyan-300/20 bg-cyan-300/10 text-cyan-200'
            }`}
          >
            #{rank + 1}
          </span>
          <ChampionPortrait
            championName={recommendation.champion}
            className={`rounded-2xl border text-sm shadow-lg ${
              rank === 0
                ? 'size-16 border-amber-300/30 shadow-amber-950/30 sm:size-20'
                : 'size-14 border-white/10 shadow-black/20 sm:size-16'
            }`}
          />
          <div className="min-w-0">
            <p
              className={`flex items-center gap-1.5 text-[10px] font-bold uppercase tracking-[0.18em] ${
                rank === 0 ? 'text-amber-300' : 'text-cyan-300/70'
              }`}
            >
              {rank === 0 && <Crown className="size-3" />}
              {rank === 0 ? 'Best Pick' : 'Recommended Pick'}
            </p>
            <h3 className="mt-1 truncate text-xl font-bold tracking-tight text-white sm:text-2xl">
              {recommendation.champion}
            </h3>
            <p className="mt-1 text-[11px] text-slate-500">Engine-ranked recommendation</p>
          </div>
        </div>
        <ScoreBadge score={recommendation.score} />
      </div>

      <div className="relative mt-6">
        <p className="mb-3 text-[10px] font-semibold uppercase tracking-[0.16em] text-slate-500">
          Score breakdown
        </p>
        <ScoreBreakdown breakdown={recommendation.scoreBreakdown} />
      </div>

      <div className="relative mt-6 grid gap-3 xl:grid-cols-2">
        <div className="rounded-2xl border border-white/8 bg-white/[0.03] p-4">
          <div className="mb-3 flex items-center gap-2 text-sm font-semibold text-white">
            <ShieldCheck className="size-4 text-cyan-300" />
            Engine Reasons
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
        <div className="mt-4 rounded-2xl border border-violet-300/20 bg-gradient-to-br from-violet-300/[0.1] to-cyan-300/[0.04] p-4 sm:p-5">
          <div className="mb-3 flex flex-wrap items-center justify-between gap-2">
            <div className="flex items-center gap-2 text-sm font-semibold text-violet-100">
              <Bot className="size-4" />
              AI Analysis
            </div>
            <span className="inline-flex items-center gap-1 rounded-full border border-violet-300/15 bg-violet-300/[0.08] px-2 py-1 text-[9px] font-semibold uppercase tracking-[0.12em] text-violet-200/70">
              <Sparkles className="size-2.5" />
              Explains engine result
            </span>
          </div>
          <p className="whitespace-pre-line text-sm leading-6 text-violet-100/75">
            {recommendation.aiExplanation}
          </p>
        </div>
      )}
    </motion.article>
  )
}
