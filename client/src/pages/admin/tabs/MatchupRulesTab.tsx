import { useEffect, useState } from 'react'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import {
  createMatchupRule,
  deleteMatchupRule,
  getMatchupRules,
  updateMatchupRule,
} from '../../../api/adminApi'
import { getApiErrorMessage } from '../../../api/api'
import AdminField from '../../../components/admin/AdminField'
import AdminModal from '../../../components/admin/AdminModal'
import type { CreateMatchupRuleRequest, MatchupRuleDto } from '../../../types'
import { ROLES } from '../../../constants/roles'

interface MatchupRulesTabProps {
  apiKey: string
}

const emptyForm: CreateMatchupRuleRequest = {
  role: ROLES[0],
  champion: '',
  enemyChampion: '',
  scoreModifier: 0,
  reason: '',
  plan: '',
}

export default function MatchupRulesTab({ apiKey }: MatchupRulesTabProps) {
  const [rules, setRules] = useState<MatchupRuleDto[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [modalOpen, setModalOpen] = useState(false)
  const [editingRule, setEditingRule] = useState<MatchupRuleDto | null>(null)
  const [form, setForm] = useState<CreateMatchupRuleRequest>(emptyForm)
  const [formError, setFormError] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)
  const [filter, setFilter] = useState('')

  useEffect(() => {
    void load()
  }, [])

  async function load() {
    setLoading(true)
    try {
      setRules(await getMatchupRules())
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }

  function openCreate() {
    setEditingRule(null)
    setForm(emptyForm)
    setFormError(null)
    setModalOpen(true)
  }

  function openEdit(rule: MatchupRuleDto) {
    setEditingRule(rule)
    setForm({
      role: rule.role,
      champion: rule.champion,
      enemyChampion: rule.enemyChampion,
      scoreModifier: rule.scoreModifier,
      reason: rule.reason,
      plan: rule.plan,
    })
    setFormError(null)
    setModalOpen(true)
  }

  async function handleSave() {
    setSaving(true)
    setFormError(null)
    try {
      if (editingRule) {
        const updated = await updateMatchupRule(editingRule.id, form, apiKey)
        setRules(prev => prev.map(r => (r.id === updated.id ? updated : r)))
      } else {
        const created = await createMatchupRule(form, apiKey)
        setRules(prev => [...prev, created])
      }
      setModalOpen(false)
    } catch (err) {
      setFormError(getApiErrorMessage(err))
    } finally {
      setSaving(false)
    }
  }

  async function handleDelete(rule: MatchupRuleDto) {
    if (!confirm(`Delete rule: ${rule.champion} vs ${rule.enemyChampion} (${rule.role})?`)) return
    try {
      await deleteMatchupRule(rule.id, apiKey)
      setRules(prev => prev.filter(r => r.id !== rule.id))
    } catch (err) {
      setError(getApiErrorMessage(err))
    }
  }

  const filtered = filter
    ? rules.filter(
        r =>
          r.champion.toLowerCase().includes(filter.toLowerCase()) ||
          r.enemyChampion.toLowerCase().includes(filter.toLowerCase()) ||
          r.role.toLowerCase().includes(filter.toLowerCase()),
      )
    : rules

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <input
          value={filter}
          onChange={e => setFilter(e.target.value)}
          placeholder="Search champion, enemy, role…"
          className="w-64 rounded-xl border border-white/10 bg-white/5 px-3 py-2 text-sm text-white placeholder-slate-500 outline-none focus:border-cyan-500/60"
        />
        <button
          onClick={openCreate}
          className="inline-flex items-center gap-2 rounded-xl bg-cyan-400/15 px-4 py-2 text-sm font-medium text-cyan-300 transition hover:bg-cyan-400/25"
        >
          <Plus className="size-4" /> Add rule
        </button>
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
                <th className="px-4 py-3">Role</th>
                <th className="px-4 py-3">Champion</th>
                <th className="px-4 py-3">Enemy</th>
                <th className="px-4 py-3 text-right">Modifier</th>
                <th className="px-4 py-3">Reason</th>
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody>
              {filtered.map(rule => (
                <tr key={rule.id} className="border-b border-white/5 hover:bg-white/3">
                  <td className="px-4 py-3 text-slate-300">{rule.role}</td>
                  <td className="px-4 py-3 font-medium text-white">{rule.champion}</td>
                  <td className="px-4 py-3 text-slate-300">{rule.enemyChampion}</td>
                  <td className="px-4 py-3 text-right">
                    <span
                      className={`font-mono font-semibold ${rule.scoreModifier > 0 ? 'text-emerald-400' : rule.scoreModifier < 0 ? 'text-red-400' : 'text-slate-400'}`}
                    >
                      {rule.scoreModifier > 0 ? '+' : ''}
                      {rule.scoreModifier}
                    </span>
                  </td>
                  <td className="max-w-[260px] truncate px-4 py-3 text-slate-400">{rule.reason}</td>
                  <td className="px-4 py-3">
                    <div className="flex justify-end gap-1">
                      <button
                        onClick={() => openEdit(rule)}
                        className="rounded-lg p-1.5 text-slate-500 transition hover:bg-white/8 hover:text-white"
                      >
                        <Pencil className="size-3.5" />
                      </button>
                      <button
                        onClick={() => void handleDelete(rule)}
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
                  <td colSpan={6} className="py-10 text-center text-slate-500">
                    No rules found.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}

      {modalOpen && (
        <AdminModal
          title={editingRule ? 'Edit matchup rule' : 'Add matchup rule'}
          onClose={() => setModalOpen(false)}
        >
          <div className="space-y-3">
            <div>
              <label className="mb-1.5 block text-xs font-medium text-slate-400">Role</label>
              <select
                value={form.role}
                onChange={e => setForm(f => ({ ...f, role: e.target.value }))}
                className="w-full rounded-xl border border-white/10 bg-white/5 px-3 py-2 text-sm text-white outline-none focus:border-cyan-500/60"
              >
                {ROLES.map(r => (
                  <option key={r} value={r} className="bg-slate-900">
                    {r}
                  </option>
                ))}
              </select>
            </div>
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
              label="Score modifier"
              type="number"
              min={-100}
              max={100}
              value={form.scoreModifier}
              onChange={e => setForm(f => ({ ...f, scoreModifier: Number(e.target.value) }))}
            />
            <AdminField
              label="Reason"
              as="textarea"
              value={form.reason}
              onChange={e => setForm(f => ({ ...f, reason: e.target.value }))}
              placeholder="Why does this rule exist?"
            />
            <AdminField
              label="Plan (optional)"
              as="textarea"
              value={form.plan ?? ''}
              onChange={e => setForm(f => ({ ...f, plan: e.target.value }))}
              placeholder="How to execute this matchup?"
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
                {saving ? 'Saving…' : editingRule ? 'Update' : 'Create'}
              </button>
            </div>
          </div>
        </AdminModal>
      )}
    </div>
  )
}
