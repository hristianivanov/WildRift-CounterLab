import type { ReactNode } from 'react'

interface SectionCardProps {
  children: ReactNode
  className?: string
}

export default function SectionCard({ children, className = '' }: SectionCardProps) {
  return (
    <section
      className={`rounded-[28px] border border-white/10 bg-slate-900/70 shadow-2xl shadow-black/25 backdrop-blur-xl ${className}`}
    >
      {children}
    </section>
  )
}
