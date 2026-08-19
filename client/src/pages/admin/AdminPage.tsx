import { useState } from 'react'
import { Link } from 'react-router-dom'
import { ChevronLeft, LogOut, RefreshCw, Scroll, Shield, Sword, Swords } from 'lucide-react'
import { useAdminAuth } from '../../hooks/useAdminAuth'
import AdminLoginPage from './AdminLoginPage'
import ChampionsTab from './tabs/ChampionsTab'
import MatchupRulesTab from './tabs/MatchupRulesTab'
import MechanicTipsTab from './tabs/MechanicTipsTab'
import SyncTab from './tabs/SyncTab'

type Tab = 'champions' | 'rules' | 'tips' | 'sync'

const TABS: { id: Tab; label: string; icon: React.ElementType }[] = [
  { id: 'champions', label: 'Champions', icon: Sword },
  { id: 'rules', label: 'Matchup Rules', icon: Swords },
  { id: 'tips', label: 'Mechanic Tips', icon: Scroll },
  { id: 'sync', label: 'Sync & Patch', icon: RefreshCw },
]

export default function AdminPage() {
  const { apiKey, isAuthenticated, login, logout } = useAdminAuth()
  const [activeTab, setActiveTab] = useState<Tab>('rules')

  if (!isAuthenticated) {
    return <AdminLoginPage onLogin={login} />
  }

  return (
    <div className="min-h-screen bg-slate-950 text-white">
      <header className="border-b border-white/8 bg-slate-900/80 backdrop-blur-xl">
        <div className="mx-auto flex max-w-7xl items-center justify-between gap-4 px-4 py-3 sm:px-6">
          <div className="flex items-center gap-3">
            <Link
              to="/"
              className="flex items-center gap-1.5 text-sm text-slate-500 transition hover:text-slate-300"
            >
              <ChevronLeft className="size-4" />
              Draft tool
            </Link>
            <span className="text-slate-700">/</span>
            <div className="flex items-center gap-2">
              <Shield className="size-4 text-cyan-400" />
              <span className="text-sm font-semibold">Admin</span>
            </div>
          </div>
          <button
            onClick={logout}
            className="inline-flex items-center gap-2 rounded-xl px-3 py-1.5 text-sm text-slate-500 transition hover:bg-white/8 hover:text-slate-300"
          >
            <LogOut className="size-3.5" /> Sign out
          </button>
        </div>
      </header>

      <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6">
        <nav className="mb-6 flex gap-1 overflow-x-auto">
          {TABS.map(({ id, label, icon: Icon }) => (
            <button
              key={id}
              onClick={() => setActiveTab(id)}
              className={`inline-flex shrink-0 items-center gap-2 rounded-xl px-4 py-2 text-sm font-medium transition ${
                activeTab === id
                  ? 'bg-cyan-400/15 text-cyan-300'
                  : 'text-slate-400 hover:bg-white/6 hover:text-slate-200'
              }`}
            >
              <Icon className="size-4" />
              {label}
            </button>
          ))}
        </nav>

        <div>
          {activeTab === 'champions' && <ChampionsTab apiKey={apiKey} />}
          {activeTab === 'rules' && <MatchupRulesTab apiKey={apiKey} />}
          {activeTab === 'tips' && <MechanicTipsTab apiKey={apiKey} />}
          {activeTab === 'sync' && <SyncTab apiKey={apiKey} />}
        </div>
      </div>
    </div>
  )
}
