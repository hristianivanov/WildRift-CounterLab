import type { ReactNode } from 'react'

import { configuredApiBaseUrl } from '../../api/api'

interface PageShellProps {
  children: ReactNode
}

export default function PageShell({ children }: PageShellProps) {
  return (
    <main className="relative min-h-screen overflow-hidden px-3 py-4 sm:px-6 sm:py-6 lg:px-8 lg:py-10">
      <div className="pointer-events-none absolute inset-0 bg-[linear-gradient(rgba(255,255,255,0.018)_1px,transparent_1px),linear-gradient(90deg,rgba(255,255,255,0.018)_1px,transparent_1px)] bg-[size:56px_56px] [mask-image:radial-gradient(circle_at_center,black,transparent_78%)]" />
      <div className="relative mx-auto max-w-[1440px]">{children}</div>
      {import.meta.env.DEV && (
        <div className="relative mx-auto mt-8 max-w-[1440px] text-right">
          <span className="inline-flex rounded-full border border-white/8 bg-black/20 px-3 py-1 text-[10px] font-medium text-slate-600">
            API: {configuredApiBaseUrl}
          </span>
        </div>
      )}
    </main>
  )
}
