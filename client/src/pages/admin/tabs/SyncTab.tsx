import { useState } from 'react'
import { RefreshCw, Zap } from 'lucide-react'
import { adminPatchCheck, adminSyncChampions } from '../../../api/adminApi'
import { getApiErrorMessage } from '../../../api/api'
import type { ChampionSyncResultDto, PatchCheckResultDto } from '../../../types'

interface SyncTabProps {
  apiKey: string
}

export default function SyncTab({ apiKey }: SyncTabProps) {
  const [syncResult, setSyncResult] = useState<ChampionSyncResultDto | null>(null)
  const [patchResult, setPatchResult] = useState<PatchCheckResultDto | null>(null)
  const [syncLoading, setSyncLoading] = useState(false)
  const [patchLoading, setPatchLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function handleSync() {
    setSyncLoading(true)
    setError(null)
    setSyncResult(null)
    try {
      const result = await adminSyncChampions(apiKey)
      setSyncResult(result)
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setSyncLoading(false)
    }
  }

  async function handlePatchCheck() {
    setPatchLoading(true)
    setError(null)
    setPatchResult(null)
    try {
      const result = await adminPatchCheck(apiKey)
      setPatchResult(result)
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setPatchLoading(false)
    }
  }

  return (
    <div className="space-y-6">
      {error && (
        <div className="rounded-xl border border-red-500/20 bg-red-500/10 px-4 py-3 text-sm text-red-400">
          {error}
        </div>
      )}

      <div className="grid gap-4 sm:grid-cols-2">
        <ActionCard
          icon={RefreshCw}
          title="Sync Champions"
          description="Fetch the latest champion list from Data Dragon and update the database."
          loading={syncLoading}
          buttonLabel="Sync now"
          onClick={handleSync}
        />
        <ActionCard
          icon={Zap}
          title="Check Patch"
          description="Check if a new Data Dragon patch is available and trigger sync if so."
          loading={patchLoading}
          buttonLabel="Check patch"
          onClick={handlePatchCheck}
        />
      </div>

      {syncResult && (
        <ResultCard title="Sync result">
          <StatRow label="Added" value={syncResult.added} />
          <StatRow label="Updated" value={syncResult.updated} />
          <StatRow label="Removed" value={syncResult.removed} />
        </ResultCard>
      )}

      {patchResult && (
        <ResultCard title="Patch check result">
          <StatRow label="Latest version" value={patchResult.latestVersion} />
          <StatRow label="Previous version" value={patchResult.previousVersion ?? '—'} />
          <StatRow label="Sync triggered" value={patchResult.syncTriggered ? 'Yes' : 'No'} />
          {patchResult.syncResult && (
            <>
              <StatRow label="Champions added" value={patchResult.syncResult.added} />
              <StatRow label="Champions updated" value={patchResult.syncResult.updated} />
              <StatRow label="Champions removed" value={patchResult.syncResult.removed} />
            </>
          )}
        </ResultCard>
      )}
    </div>
  )
}

function ActionCard({
  icon: Icon,
  title,
  description,
  loading,
  buttonLabel,
  onClick,
}: {
  icon: React.ElementType
  title: string
  description: string
  loading: boolean
  buttonLabel: string
  onClick: () => void
}) {
  return (
    <div className="rounded-2xl border border-white/8 bg-slate-800/50 p-5">
      <div className="mb-3 flex size-10 items-center justify-center rounded-xl border border-cyan-400/20 bg-cyan-400/10">
        <Icon className="size-5 text-cyan-300" />
      </div>
      <h3 className="font-semibold text-white">{title}</h3>
      <p className="mt-1 text-sm text-slate-400">{description}</p>
      <button
        onClick={onClick}
        disabled={loading}
        className="mt-4 inline-flex items-center gap-2 rounded-xl bg-white/8 px-4 py-2 text-sm font-medium text-slate-200 transition hover:bg-white/12 disabled:cursor-not-allowed disabled:opacity-40"
      >
        {loading ? (
          <RefreshCw className="size-3.5 animate-spin" />
        ) : (
          <Icon className="size-3.5" />
        )}
        {loading ? 'Running…' : buttonLabel}
      </button>
    </div>
  )
}

function ResultCard({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div className="rounded-2xl border border-white/8 bg-slate-800/50 p-5">
      <h3 className="mb-3 text-sm font-semibold text-slate-300">{title}</h3>
      <div className="space-y-1.5">{children}</div>
    </div>
  )
}

function StatRow({ label, value }: { label: string; value: string | number }) {
  return (
    <div className="flex justify-between text-sm">
      <span className="text-slate-400">{label}</span>
      <span className="font-medium text-white">{value}</span>
    </div>
  )
}
