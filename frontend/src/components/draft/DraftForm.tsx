import { LoaderCircle, Sparkles } from 'lucide-react'

import type { Champion, DraftRecommendationRequest } from '../../types'
import Button from '../common/Button'

const roles = ['Baron', 'Jungle', 'Mid', 'Dragon', 'Support']

interface DraftFormProps {
  champions: Champion[]
  value: DraftRecommendationRequest
  isLoading: boolean
  usingFallback: boolean
  onChange: (value: DraftRecommendationRequest) => void
  onSubmit: () => void
}

const fieldClass =
  'w-full rounded-xl border border-white/10 bg-slate-950/70 px-3 py-3 text-sm text-slate-100 outline-none transition focus:border-cyan-400/60'

export default function DraftForm({
  champions,
  value,
  isLoading,
  usingFallback,
  onChange,
  onSubmit,
}: DraftFormProps) {
  const selectedEnemies = new Set([value.laneEnemy, ...value.enemyTeam])

  function toggleEnemy(name: string) {
    const exists = value.enemyTeam.includes(name)

    onChange({
      ...value,
      enemyTeam: exists
        ? value.enemyTeam.filter((enemy) => enemy !== name)
        : value.enemyTeam.length < 4
          ? [...value.enemyTeam, name]
          : value.enemyTeam,
    })
  }

  return (
    <section className="rounded-3xl border border-white/10 bg-slate-900/70 p-5 shadow-2xl shadow-black/30 backdrop-blur md:p-7">
      <div className="mb-6 flex items-start justify-between gap-4">
        <div>
          <p className="text-xs font-semibold uppercase tracking-[0.22em] text-cyan-300">
            Draft inputs
          </p>
          <h2 className="mt-2 text-xl font-semibold text-white">Build the enemy draft</h2>
        </div>
        {usingFallback && (
          <span className="rounded-full border border-amber-300/20 bg-amber-300/10 px-3 py-1 text-xs text-amber-200">
            Offline champion fallback
          </span>
        )}
      </div>

      <div className="grid gap-4 sm:grid-cols-2">
        <label className="space-y-2 text-sm text-slate-300">
          <span>Role</span>
          <select
            className={fieldClass}
            value={value.role}
            onChange={(event) => onChange({ ...value, role: event.target.value })}
          >
            {roles.map((role) => (
              <option key={role} value={role}>
                {role}
              </option>
            ))}
          </select>
        </label>

        <label className="space-y-2 text-sm text-slate-300">
          <span>Lane enemy</span>
          <select
            className={fieldClass}
            value={value.laneEnemy}
            onChange={(event) =>
              onChange({
                ...value,
                laneEnemy: event.target.value,
                enemyTeam: value.enemyTeam.filter((name) => name !== event.target.value),
              })
            }
          >
            <option value="">Select a champion</option>
            {champions.map((champion) => (
              <option key={champion.id || champion.name} value={champion.name}>
                {champion.name}
              </option>
            ))}
          </select>
        </label>
      </div>

      <div className="mt-5">
        <div className="mb-3 flex items-center justify-between">
          <span className="text-sm text-slate-300">Enemy team</span>
          <span className="text-xs text-slate-500">{value.enemyTeam.length}/4 extra picks</span>
        </div>
        <div className="flex max-h-48 flex-wrap gap-2 overflow-y-auto pr-1">
          {champions.map((champion) => {
            const selected = value.enemyTeam.includes(champion.name)
            const unavailable = champion.name === value.laneEnemy

            return (
              <button
                key={champion.id || champion.name}
                type="button"
                disabled={unavailable || (!selected && value.enemyTeam.length >= 4)}
                onClick={() => toggleEnemy(champion.name)}
                className={`rounded-full border px-3 py-1.5 text-xs transition ${
                  selected
                    ? 'border-cyan-300/60 bg-cyan-300/15 text-cyan-100'
                    : selectedEnemies.has(champion.name)
                      ? 'border-white/5 bg-white/5 text-slate-600'
                      : 'border-white/10 bg-white/5 text-slate-300 hover:border-white/25 hover:text-white'
                } disabled:cursor-not-allowed disabled:opacity-40`}
              >
                {champion.name}
              </button>
            )
          })}
        </div>
      </div>

      <label className="mt-6 flex cursor-pointer items-center gap-3 rounded-xl border border-white/10 bg-white/[0.03] p-4">
        <input
          type="checkbox"
          checked={value.includeAiExplanation}
          onChange={(event) =>
            onChange({ ...value, includeAiExplanation: event.target.checked })
          }
          className="size-4 accent-cyan-400"
        />
        <span>
          <span className="block text-sm font-medium text-white">Include AI explanation</span>
          <span className="block text-xs text-slate-500">
            Adds human-readable explanations after deterministic scoring.
          </span>
        </span>
      </label>

      <Button
        type="button"
        className="mt-6 w-full"
        disabled={isLoading || !value.laneEnemy}
        onClick={onSubmit}
      >
        {isLoading ? (
          <>
            <LoaderCircle className="size-4 animate-spin" />
            Analyzing draft
          </>
        ) : (
          <>
            <Sparkles className="size-4" />
            Analyze Draft
          </>
        )}
      </Button>
    </section>
  )
}
