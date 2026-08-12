const FALLBACK_VERSION = '15.1.1'

// Lazily resolved — kicks off on first import, resolves before any image renders in practice.
const latestVersionPromise: Promise<string> = fetch(
  'https://ddragon.leagueoflegends.com/api/versions.json',
)
  .then((r) => r.json())
  .then((versions: string[]) => versions[0] ?? FALLBACK_VERSION)
  .catch(() => FALLBACK_VERSION)

let resolvedVersion: string | null = null
latestVersionPromise.then((v) => {
  resolvedVersion = v
})

function getVersion(): string {
  return resolvedVersion ?? FALLBACK_VERSION
}

// Keys that differ from simply stripping non-alphanumeric characters from the display name.
// Data Dragon champion keys are case-sensitive and sometimes differ from the display name.
const championImageKeys: Record<string, string> = {
  'dr. mundo': 'DrMundo',
  "kai'sa": 'Kaisa',
  "kha'zix": 'Khazix',
  "vel'koz": 'Velkoz',
  "cho'gath": 'Chogath',
  "rek'sai": 'RekSai',
  "k'sante": 'KSante',
  "kog'maw": 'KogMaw',
  'nunu & willump': 'Nunu',
  wukong: 'MonkeyKing',
  'master yi': 'MasterYi',
  'jarvan iv': 'JarvanIV',
  'aurelion sol': 'AurelionSol',
  'twisted fate': 'TwistedFate',
  'miss fortune': 'MissFortune',
  'xin zhao': 'XinZhao',
}

// Champions exclusive to Wild Rift that don't exist in PC LoL Data Dragon.
// Images are served from Community Dragon instead.
const cdagonWrKeys: Record<string, string> = {
  norra: 'norra',
  yunara: 'yunara',
}

export function getChampionImageUrl(championName: string): string {
  const normalizedName = championName.trim().toLowerCase()

  if (cdagonWrKeys[normalizedName]) {
    const key = cdagonWrKeys[normalizedName]
    return `https://raw.communitydragon.org/wildrift/plugins/rcp-be-lol-game-data/global/default/v1/champions/${key}/hud/${key}_square.png`
  }

  const imageKey =
    championImageKeys[normalizedName] ??
    championName.replace(/[^a-zA-Z0-9]/g, '')

  return `https://ddragon.leagueoflegends.com/cdn/${getVersion()}/img/champion/${imageKey}.png`
}

export function getChampionInitials(championName: string): string {
  return championName
    .replace(/[.'&]/g, ' ')
    .split(/\s+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0])
    .join('')
    .toUpperCase()
}
