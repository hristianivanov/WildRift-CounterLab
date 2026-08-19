import { useState } from 'react'
import { Trash2 } from 'lucide-react'
import { adminDeleteChampion } from '../../../api/adminApi'
import { getApiErrorMessage } from '../../../api/api'
import { useChampions } from '../../../hooks/useChampions'
import type { Champion } from '../../../types'

interface ChampionsTabProps {
  apiKey: string
}

export default function ChampionsTab({ apiKey }: ChampionsTabProps) {
  const { champions, loading } = useChampions()
  const [localChampions, setLocalChampions] = useState<Champion[] | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [filter, setFilter] = useState('')

  const displayed = localChampions ?? champions
  const filtered = filter
    ? displayed.filter(c => c.name.toLowerCase().includes(filter.toLowerCase()))
    : displayed

  async function handleDelete(champion: Champion) {
    if (!confirm(`Delete champion "${champion.name}"? This will also remove all associated rules and tips.`)) return
    try {
      await adminDeleteChampion(champion.id, apiKey)
      setLocalChampions(prev => (prev ?? champions).filter(c => c.id !== champion.id))
    } catch (err) {
      setError(getApiErrorMessage(err))
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <input
          value={filter}
          onChange={e => setFilter(e.target.value)}
          placeholder="Search champion…"
          className="w-64 rounded-xl border border-white/10 bg-white/5 px-3 py-2 text-sm text-white placeholder-slate-500 outline-none focus:border-cyan-500/60"
        />
        <span className="text-xs text-slate-500">
          {displayed.length} champions · managed via sync
        </span>
      </div>

      {error && (
        <div className="rounded-xl border border-red-500/20 bg-red-500/10 px-4 py-3 text-sm text-red-400">
          {error}
        </div>
      )}

      {loading ? (
        <p className="py-8 text-center text-sm text-slate-500">Loading…</p>
      ) : (
        <div className="overflow-x-auto rounded-2xl border border-white/8">
          <table className="w-full min-w-[480px] text-sm">
            <thead>
              <tr className="border-b border-white/8 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">
                <th className="px-4 py-3">Champion</th>
                <th className="px-4 py-3">Roles</th>
                <th className="px-4 py-3">Tags</th>
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody>
              {filtered.map(champion => (
                <tr key={champion.id} className="border-b border-white/5 hover:bg-white/3">
                  <td className="px-4 py-3 font-medium text-white">{champion.name}</td>
                  <td className="px-4 py-3 text-slate-400">{champion.roles.join(', ')}</td>
                  <td className="px-4 py-3 text-slate-500">{champion.tags.join(', ')}</td>
                  <td className="px-4 py-3">
                    <div className="flex justify-end">
                      <button
                        onClick={() => void handleDelete(champion)}
                        className="rounded-lg p-1.5 text-slate-500 transition hover:bg-red-500/15 hover:text-red-400"
                        title="Delete champion"
                      >
                        <Trash2 className="size-3.5" />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
              {filtered.length === 0 && (
                <tr>
                  <td colSpan={4} className="py-10 text-center text-slate-500">
                    No champions found.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
