interface ScoreBadgeProps {
  score: number
}

export default function ScoreBadge({ score }: ScoreBadgeProps) {
  const tone =
    score >= 80
      ? 'from-emerald-300 to-cyan-300 text-emerald-100'
      : score >= 65
        ? 'from-cyan-300 to-blue-400 text-cyan-100'
        : 'from-amber-300 to-orange-400 text-amber-100'

  return (
    <div className="relative grid size-16 shrink-0 place-items-center rounded-full bg-slate-950 p-1 shadow-xl shadow-black/30 sm:size-[76px]">
      <div className={`absolute inset-0 rounded-full bg-gradient-to-br ${tone} opacity-80`} />
      <div className="relative grid size-full place-items-center rounded-full bg-slate-950">
        <div className="text-center">
          <p className={`text-xl font-black leading-none sm:text-2xl ${tone.split(' ').at(-1)}`}>
            {score}
          </p>
          <p className="mt-1 text-[9px] font-semibold uppercase tracking-[0.16em] text-slate-500">
            score
          </p>
        </div>
      </div>
    </div>
  )
}
