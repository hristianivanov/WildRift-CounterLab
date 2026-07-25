import { Crosshair } from 'lucide-react'

interface EmptyStateProps {
  title?: string
  message?: string
}

export default function EmptyState({
  title = 'Recommendations appear here',
  message = 'Choose a lane opponent and enemy draft, then run the analysis.',
}: EmptyStateProps) {
  return (
    <div className="grid min-h-80 place-items-center rounded-[28px] border border-dashed border-white/10 bg-slate-900/35 p-8 text-center">
      <div className="max-w-sm">
        <span className="mx-auto grid size-14 place-items-center rounded-2xl border border-cyan-300/15 bg-cyan-300/[0.07] text-cyan-300">
          <Crosshair className="size-6" />
        </span>
        <p className="mt-5 text-lg font-semibold text-slate-200">{title}</p>
        <p className="mt-2 text-sm leading-6 text-slate-500">{message}</p>
      </div>
    </div>
  )
}
