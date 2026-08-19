import { api } from './api'
import type {
  MatchupRuleDto,
  CreateMatchupRuleRequest,
  UpdateMatchupRuleRequest,
  MatchupTipDto,
  CreateMatchupTipRequest,
  UpdateMatchupTipRequest,
  ChampionSyncResultDto,
  PatchCheckResultDto,
} from '../types'

function authHeaders(apiKey: string) {
  return { headers: { 'X-Api-Key': apiKey } }
}

// Champions
export async function adminSyncChampions(apiKey: string): Promise<ChampionSyncResultDto> {
  const { data } = await api.post<ChampionSyncResultDto>('/champions/sync', null, authHeaders(apiKey))
  return data
}

export async function adminPatchCheck(apiKey: string): Promise<PatchCheckResultDto> {
  const { data } = await api.post<PatchCheckResultDto>('/champions/patch-check', null, authHeaders(apiKey))
  return data
}

export async function adminDeleteChampion(id: number, apiKey: string): Promise<void> {
  await api.delete(`/champions/${id}`, authHeaders(apiKey))
}

// Matchup Rules
export async function getMatchupRules(): Promise<MatchupRuleDto[]> {
  const { data } = await api.get<MatchupRuleDto[]>('/matchup-rules')
  return data
}

export async function createMatchupRule(
  request: CreateMatchupRuleRequest,
  apiKey: string,
): Promise<MatchupRuleDto> {
  const { data } = await api.post<MatchupRuleDto>('/matchup-rules', request, authHeaders(apiKey))
  return data
}

export async function updateMatchupRule(
  id: number,
  request: UpdateMatchupRuleRequest,
  apiKey: string,
): Promise<MatchupRuleDto> {
  const { data } = await api.put<MatchupRuleDto>(`/matchup-rules/${id}`, request, authHeaders(apiKey))
  return data
}

export async function deleteMatchupRule(id: number, apiKey: string): Promise<void> {
  await api.delete(`/matchup-rules/${id}`, authHeaders(apiKey))
}

// Matchup Tips
export async function getMatchupTips(): Promise<MatchupTipDto[]> {
  const { data } = await api.get<MatchupTipDto[]>('/matchup-tips')
  return data
}

export async function createMatchupTip(
  request: CreateMatchupTipRequest,
  apiKey: string,
): Promise<MatchupTipDto> {
  const { data } = await api.post<MatchupTipDto>('/matchup-tips', request, authHeaders(apiKey))
  return data
}

export async function updateMatchupTip(
  id: number,
  request: UpdateMatchupTipRequest,
  apiKey: string,
): Promise<MatchupTipDto> {
  const { data } = await api.put<MatchupTipDto>(`/matchup-tips/${id}`, request, authHeaders(apiKey))
  return data
}

export async function deleteMatchupTip(id: number, apiKey: string): Promise<void> {
  await api.delete(`/matchup-tips/${id}`, authHeaders(apiKey))
}
