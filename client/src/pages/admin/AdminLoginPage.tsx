import { type FormEvent, useState } from 'react'
import { KeyRound, ShieldCheck } from 'lucide-react'

interface AdminLoginPageProps {
  onLogin: (key: string) => void
}

export default function AdminLoginPage({ onLogin }: AdminLoginPageProps) {
  const [key, setKey] = useState('')

  function handleSubmit(e: FormEvent) {
    e.preventDefault()
    const trimmed = key.trim()
    if (trimmed) onLogin(trimmed)
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-950 p-4">
      <div className="w-full max-w-sm">
        <div className="mb-8 flex flex-col items-center gap-3 text-center">
          <div className="flex size-14 items-center justify-center rounded-2xl border border-cyan-400/20 bg-cyan-400/10">
            <ShieldCheck className="size-7 text-cyan-300" />
          </div>
          <div>
            <h1 className="text-xl font-bold text-white">Admin Panel</h1>
            <p className="mt-1 text-sm text-slate-500">WildRift CounterLab</p>
          </div>
        </div>

        <form
          onSubmit={handleSubmit}
          className="rounded-2xl border border-white/10 bg-slate-900/80 p-6 shadow-2xl shadow-black/30"
        >
          <label className="mb-1.5 block text-xs font-medium text-slate-400">
            <KeyRound className="mb-0.5 mr-1 inline size-3" />
            API Key
          </label>
          <input
            type="password"
            autoFocus
            value={key}
            onChange={e => setKey(e.target.value)}
            placeholder="Enter admin API key"
            className="w-full rounded-xl border border-white/10 bg-white/5 px-3 py-2.5 text-sm text-white placeholder-slate-600 outline-none transition focus:border-cyan-500/60 focus:bg-white/8"
          />
          <button
            type="submit"
            disabled={!key.trim()}
            className="mt-4 w-full rounded-xl bg-gradient-to-r from-cyan-400 to-cyan-500 py-2.5 text-sm font-bold text-slate-950 shadow-lg shadow-cyan-500/20 transition hover:from-cyan-300 hover:to-cyan-400 disabled:cursor-not-allowed disabled:opacity-40"
          >
            Sign in
          </button>
        </form>
      </div>
    </div>
  )
}
