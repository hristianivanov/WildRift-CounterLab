import { WifiOff } from 'lucide-react'

interface ErrorStateProps {
  message: string
}

export default function ErrorState({ message }: ErrorStateProps) {
  return (
    <div className="mb-5 flex items-start gap-3 rounded-2xl border border-red-300/20 bg-red-300/[0.08] p-4 text-sm text-red-100 shadow-lg shadow-red-950/10">
      <span className="grid size-9 shrink-0 place-items-center rounded-xl bg-red-300/10">
        <WifiOff className="size-4" />
      </span>
      <div>
        <p className="font-semibold">Analysis unavailable</p>
        <p className="mt-1 leading-5 text-red-100/70">{message}</p>
      </div>
    </div>
  )
}
