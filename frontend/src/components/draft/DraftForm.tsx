import { Bot, Info, LoaderCircle, Sparkles, X } from 'lucide-react'

import type { Champion, DraftRecommendationRequest } from '../../types'
import Button from '../common/Button'
import ChampionPortrait from '../common/ChampionPortrait'
import SectionCard from '../common/SectionCard'

const roles = ['Baron', 'Jungle', 'Mid', 'Dragon', 'Support']

interface DraftFormProps {
  champions: Champion[]
  value: DraftRecommendationRequest
  isLoading: boolean
  aiEnabled: boolean
  usingFallback: boolean
  onChange: (value: DraftRecommendationRequest) => void
  onSubmit: () => void
}

const fieldClass =
  'w-full appearance-none rounded-xl border border-white/10 bg-slate-950/75 px-3 py-3 text-sm text-slate-100 outline-none transition hover:border-white/20 focus:border-cyan-400/60 focus:ring-2 focus:ring-cyan-400/10'

export default function DraftForm({
  champions,
  value,
  isLoading,
  aiEnabled,
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
    <SectionCard className="p-5 md:p-7 lg:sticky lg:top-6">
      <div className="mb-6 flex items-start justify-between gap-4">
        <div>
          <p className="text-xs font-semibold uppercase tracking-[0.22em] text-cyan-300">
            Draft setup
          </p>
          <h2 className="mt-2 text-xl font-bold text-white">Build the matchup</h2>
          <p className="mt-1 text-xs leading-5 text-slate-500">
            Start with your lane, then add the visible enemy picks.
          </p>
        </div>
      </div>

      {usingFallback && (
        <div className="mb-5 flex items-start gap-3 rounded-xl border border-amber-300/20 bg-amber-300/[0.08] p-3 text-xs text-amber-100">
          <Info className="mt-0.5 size-4 shrink-0 text-amber-300" />
          <span>
            The backend could not be reached, so a limited fallback champion list is active.
          </span>
        </div>
      )}

      <div>
        <p className="mb-2 text-xs font-semibold uppercase tracking-[0.14em] text-slate-500">
          Your role
        </p>
        <div className="grid grid-cols-3 gap-2 sm:grid-cols-5 lg:grid-cols-3 xl:grid-cols-5">
          {roles.map((role) => (
            <button
              key={role}
              type="button"
              onClick={() => onChange({ ...value, role })}
              aria-pressed={value.role === role}
              aria-label={`Select ${role} role`}
              className={`rounded-xl border px-2 py-2.5 text-xs font-semibold transition ${
                value.role === role
                  ? 'border-cyan-300/50 bg-cyan-300/15 text-cyan-100 shadow-sm shadow-cyan-500/10'
                  : 'border-white/8 bg-white/[0.025] text-slate-400 hover:border-white/20 hover:text-slate-200'
              }`}
            >
              {role}
            </button>
          ))}
        </div>
      </div>

      <div className="mt-5">
        <label className="space-y-2 text-sm text-slate-300">
          <span className="text-xs font-semibold uppercase tracking-[0.14em] text-slate-500">
            Lane enemy
          </span>
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
        {value.laneEnemy && (
          <div className="mt-3 flex items-center gap-3 rounded-xl border border-white/8 bg-black/15 p-2.5">
            <ChampionPortrait
              championName={value.laneEnemy}
              className="size-10 rounded-xl border border-white/10 text-xs"
            />
            <div className="min-w-0">
              <p className="text-[10px] font-semibold uppercase tracking-[0.14em] text-slate-500">
                Lane opponent
              </p>
              <p className="truncate text-sm font-semibold text-slate-200">{value.laneEnemy}</p>
            </div>
          </div>
        )}
      </div>

      <div className="mt-6">
        <div className="mb-3 flex items-center justify-between">
          <span className="text-xs font-semibold uppercase tracking-[0.14em] text-slate-500">
            Enemy team
          </span>
          <span className="rounded-full bg-white/[0.04] px-2 py-1 text-[10px] font-semibold text-slate-500">
            {value.enemyTeam.length}/4 added
          </span>
        </div>

        {value.enemyTeam.length > 0 && (
          <div className="mb-3 flex flex-wrap gap-2 rounded-xl border border-cyan-300/10 bg-cyan-300/[0.04] p-3">
            {value.enemyTeam.map((enemy) => (
              <button
                key={enemy}
                type="button"
                onClick={() => toggleEnemy(enemy)}
                aria-label={`Remove ${enemy} from enemy team`}
                className="inline-flex max-w-full items-center gap-1.5 rounded-lg border border-cyan-300/25 bg-cyan-300/10 py-1 pr-2.5 pl-1 text-xs font-medium text-cyan-100 transition hover:bg-cyan-300/20"
              >
                <ChampionPortrait championName={enemy} className="size-6 rounded-md text-[8px]" />
                <span className="truncate">{enemy}</span>
                <X className="size-3" />
              </button>
            ))}
          </div>
        )}

        <div className="flex max-h-44 flex-wrap gap-2 overflow-y-auto pr-1">
          {champions.map((champion) => {
            const selected = value.enemyTeam.includes(champion.name)
            const unavailable = champion.name === value.laneEnemy

            return (
              <button
                key={champion.id || champion.name}
                type="button"
                disabled={unavailable || (!selected && value.enemyTeam.length >= 4)}
                onClick={() => toggleEnemy(champion.name)}
                aria-pressed={selected}
                aria-label={`${selected ? 'Remove' : 'Add'} ${champion.name} ${selected ? 'from' : 'to'} enemy team`}
                className={`inline-flex items-center gap-1.5 rounded-lg border py-1 pr-2.5 pl-1 text-xs transition ${
                  selected
                    ? 'border-cyan-300/25 bg-cyan-300/10 text-cyan-100'
                    : selectedEnemies.has(champion.name)
                      ? 'border-white/5 bg-white/5 text-slate-600'
                      : 'border-white/10 bg-white/5 text-slate-300 hover:border-white/25 hover:text-white'
                } disabled:cursor-not-allowed disabled:opacity-40`}
              >
                <ChampionPortrait
                  championName={champion.name}
                  className="size-6 rounded-md text-[8px]"
                />
                {champion.name}
              </button>
            )
          })}
        </div>
      </div>

      <label
        className={`mt-6 flex items-start gap-3 rounded-xl border border-violet-300/10 bg-violet-300/[0.04] p-4 transition ${
          aiEnabled ? 'cursor-pointer hover:border-violet-300/20' : 'cursor-not-allowed opacity-70'
        }`}
      >
        <input
          type="checkbox"
          disabled={!aiEnabled}
          checked={value.includeAiExplanation}
          onChange={(event) =>
            onChange({ ...value, includeAiExplanation: event.target.checked })
          }
          className="mt-0.5 size-4 accent-violet-400"
        />
        <Bot className="mt-0.5 size-4 shrink-0 text-violet-300" />
        <span className="min-w-0">
          <span className="block text-sm font-semibold text-white">Include AI explanation</span>
          <span className="mt-1 block text-xs leading-5 text-slate-500">
            {aiEnabled
              ? 'Recommendations appear first, then AI analysis fills in for the best pick.'
              : 'AI analysis disabled for demo stability.'}
          </span>
        </span>
      </label>

      <Button
        type="button"
        className="mt-6 w-full py-3.5"
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
    </SectionCard>
  )
}
