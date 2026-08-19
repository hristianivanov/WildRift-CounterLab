import { useEffect, useState } from 'react'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import {
  createMatchupTip,
  deleteMatchupTip,
  getMatchupTips,
  updateMatchupTip,
} from '../../../api/adminApi'
import { getApiErrorMessage } from '../../../api/api'
import AdminField from '../../../components/admin/AdminField'
import AdminModal from '../../../components/admin/AdminModal'
import type { CreateMatchupTipRequest, MatchupTipDto } from '../../../types'

interface MechanicTipsTabProps {
  apiKey: string
}

const emptyForm: CreateMatchupTipRequest = {
  champion: '',
  enemyChampion: '',
  tip: '',
  abilityTag: '',
}

export default function MechanicTipsTab({ apiKey }: MechanicTipsTabProps) {
  const [tips, setTips] = useState<MatchupTipDto[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [modalOpen, setModalOpen] = useState(false)
  const [editingTip, setEditingTip] = useState<MatchupTipDto | null>(null)
  const [form, setForm] = useState<CreateMatchupTipRequest>(emptyForm)
  const [formError, setFormError] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)
  const [filter, setFilter] = useState('')

  useEffect(() => {
    void load()
  }, [])

  async function load() {
    setLoading(true)
    try {
      setTips(await getMatchupTips())
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }

  function openCreate() {
    setEditingTip(null)
    setForm(emptyForm)
    setFormError(null)
    setModalOpen(true)
  }

  function openEdit(tip: MatchupTipDto) {
    setEditingTip(tip)
    setForm({
      champion: tip.champion,
      enemyChampion: tip.enemyChampion,
      tip: tip.tip,
      abilityTag: tip.abilityTag ?? '',
    })
    setFormError(null)
    setModalOpen(true)
  }

  async function handleSave() {
    setSaving(true)
    setFormError(null)
    try {
      const payload = { ...form, abilityTag: form.abilityTag?.trim() || undefined }
      if (editingTip) {
        const updated = await updateMatchupTip(editingTip.id, payload, apiKey)
        setTips(prev => prev.map(t => (t.id === updated.id ? updated : t)))
      } else {
        const created = await createMatchupTip(payload, apiKey)
        setTips(prev => [...prev, created])
      }
      setModalOpen(false)
    } catch (err) {
      setFormError(getApiErrorMessage(err))
    } finally {
      setSaving(false)
    }
  }

  async function handleDelete(tip: MatchupTipDto) {
    if (!confirm(`Delete tip for ${tip.champion} vs ${tip.enemyChampion}?`)) return
    try {
      await deleteMatchupTip(tip.id, apiKey)
      setTips(prev => prev.filter(t => t.id !== tip.id))
    } catch (err) {
      setError(getApiErrorMessage(err))
    }
  }

  const filtered = filter
    ? tips.filter(
        t =>
          t.champion.toLowerCase().includes(filter.toLowerCase()) ||
          t.enemyChampion.toLowerCase().includes(filter.toLowerCase()) ||
          t.tip.toLowerCase().includes(filter.toLowerCase()),
      )
    : tips

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <input
          value={filter}
          onChange={e => setFilter(e.target.value)}
          placeholder="Search champion, enemy, tip…"
          className="w-64 rounded-xl border border-white/10 bg-white/5 px-3 py-2 text-sm text-white placeholder-slate-500 outline-none focus:border-cyan-500/60"
        />
        <div className="flex items-center gap-2">
          <span className="text-xs text-slate-500">{tips.length} tips total</span>
          <button
            onClick={openCreate}
            className="inline-flex items-center gap-2 rounded-xl bg-cyan-400/15 px-4 py-2 text-sm font-medium text-cyan-300 transition hover:bg-cyan-400/25"
          >
            <Plus className="size-4" /> Add tip
          </button>
        </div>
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
          <table className="w-full min-w-[640px] text-sm">
            <thead>
              <tr className="border-b border-white/8 text-left text-xs font-semibold uppercase tracking-wider text-slate-500">
                <th className="px-4 py-3">Champion</th>
                <th className="px-4 py-3">Enemy</th>
                <th className="px-4 py-3">Ability</th>
                <th className="px-4 py-3">Tip</th>
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody>
              {filtered.map(tip => (
                <tr key={tip.id} className="border-b border-white/5 hover:bg-white/3">
                  <td className="px-4 py-3 font-medium text-white">{tip.champion}</td>
                  <td className="px-4 py-3 text-slate-300">{tip.enemyChampion}</td>
                  <td className="px-4 py-3">
                    {tip.abilityTag ? (
                      <span className="rounded-md bg-yellow-400/10 px-2 py-0.5 text-xs font-medium text-yellow-300">
                        {tip.abilityTag}
                      </span>
                    ) : (
                      <span className="text-slate-600">—</span>
                    )}
                  </td>
                  <td className="max-w-[340px] px-4 py-3 text-slate-400">
                    <p className="line-clamp-2">{tip.tip}</p>
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex justify-end gap-1">
                      <button
                        onClick={() => openEdit(tip)}
                        className="rounded-lg p-1.5 text-slate-500 transition hover:bg-white/8 hover:text-white"
                      >
                        <Pencil className="size-3.5" />
                      </button>
                      <button
                        onClick={() => void handleDelete(tip)}
                        className="rounded-lg p-1.5 text-slate-500 transition hover:bg-red-500/15 hover:text-red-400"
                      >
                        <Trash2 className="size-3.5" />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
              {filtered.length === 0 && (
                <tr>
                  <td colSpan={5} className="py-10 text-center text-slate-500">
                    No tips found.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}

      {modalOpen && (
        <AdminModal
          title={editingTip ? 'Edit mechanic tip' : 'Add mechanic tip'}
          onClose={() => setModalOpen(false)}
        >
          <div className="space-y-3">
            <AdminField
              label="Champion"
              value={form.champion}
              onChange={e => setForm(f => ({ ...f, champion: e.target.value }))}
              placeholder="e.g. Galio"
            />
            <AdminField
              label="Enemy champion"
              value={form.enemyChampion}
              onChange={e => setForm(f => ({ ...f, enemyChampion: e.target.value }))}
              placeholder="e.g. Veigar"
            />
            <AdminField
              label="Ability tag (optional)"
              value={form.abilityTag ?? ''}
              onChange={e => setForm(f => ({ ...f, abilityTag: e.target.value }))}
              placeholder="e.g. W, Q, Passive"
            />
            <AdminField
              label="Tip"
              as="textarea"
              value={form.tip}
              onChange={e => setForm(f => ({ ...f, tip: e.target.value }))}
              placeholder="Describe the mechanic or tip…"
            />

            {formError && <p className="text-sm text-red-400">{formError}</p>}

            <div className="flex justify-end gap-2 pt-1">
              <button
                onClick={() => setModalOpen(false)}
                className="rounded-xl px-4 py-2 text-sm text-slate-400 transition hover:bg-white/8 hover:text-white"
              >
                Cancel
              </button>
              <button
                onClick={() => void handleSave()}
                disabled={saving}
                className="rounded-xl bg-cyan-400/15 px-4 py-2 text-sm font-medium text-cyan-300 transition hover:bg-cyan-400/25 disabled:opacity-40"
              >
                {saving ? 'Saving…' : editingTip ? 'Update' : 'Create'}
              </button>
            </div>
          </div>
        </AdminModal>
      )}
    </div>
  )
}
