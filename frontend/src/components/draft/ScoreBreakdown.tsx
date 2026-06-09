import type { ScoreBreakdown as ScoreBreakdownType } from '../../types'

interface ScoreBreakdownProps {
  breakdown: ScoreBreakdownType
}

export default function ScoreBreakdown({ breakdown }: ScoreBreakdownProps) {
  const categories = [
    ['Lane', breakdown.laneScore],
    ['Team', breakdown.teamScore],
    ['Role Fit', breakdown.roleFitScore],
    ['Safety', breakdown.safetyScore],
    ['Scaling', breakdown.scalingScore],
    ...(breakdown.damageProfileScore === undefined
      ? []
      : [['Damage', breakdown.damageProfileScore] as const]),
    ['Utility', breakdown.utilityScore],
  ] as const

  const maxValue = Math.max(...categories.map(([, score]) => Math.abs(score)), 1)

  return (
    <div className="grid grid-cols-2 gap-2 sm:grid-cols-3 xl:grid-cols-6">
      {categories.map(([label, score]) => {
        const width = Math.max((Math.abs(score) / maxValue) * 100, score === 0 ? 0 : 10)
        const positive = score >= 0

        return (
          <div key={label} className="rounded-xl border border-white/8 bg-black/20 p-3">
            <div className="flex items-center justify-between gap-2">
              <p className="text-[10px] font-semibold uppercase tracking-[0.14em] text-slate-500">
                {label}
              </p>
              <p className={`text-sm font-bold ${positive ? 'text-cyan-200' : 'text-red-300'}`}>
                {score > 0 ? '+' : ''}
                {score}
              </p>
            </div>
            <div className="mt-2 h-1.5 overflow-hidden rounded-full bg-white/[0.06]">
              <div
                className={`h-full rounded-full ${positive ? 'bg-gradient-to-r from-cyan-500 to-cyan-300' : 'bg-red-400'}`}
                style={{ width: `${width}%` }}
              />
            </div>
          </div>
        )
      })}
    </div>
  )
}
