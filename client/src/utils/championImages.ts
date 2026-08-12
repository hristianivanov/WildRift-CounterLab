// Update this when a new LoL season releases and the version increments.
const DATA_DRAGON_VERSION = '16.16.1'

// Champions that exist in Wild Rift but not in PC Data Dragon — always fall back to initials.
const noImageChampions = new Set(['norra'])

// Keys that differ from simply stripping non-alphanumeric characters from the display name.
// Data Dragon champion keys are case-sensitive and sometimes differ from the display name.
const championImageKeys: Record<string, string> = {
  'aurelion sol': 'AurelionSol',
  "cho'gath": 'Chogath',
  'dr. mundo': 'DrMundo',
  'jarvan iv': 'JarvanIV',
  "kai'sa": 'Kaisa',
  "k'sante": 'KSante',
  "kha'zix": 'Khazix',
  "kog'maw": 'KogMaw',
  'master yi': 'MasterYi',
  'miss fortune': 'MissFortune',
  'nunu & willump': 'Nunu',
  "rek'sai": 'RekSai',
  "vel'koz": 'Velkoz',
  wukong: 'MonkeyKing',
  'xin zhao': 'XinZhao',
  'twisted fate': 'TwistedFate',
}

export function getChampionImageUrl(championName: string): string | null {
  const normalizedName = championName.trim().toLowerCase()
  if (noImageChampions.has(normalizedName)) return null
  const imageKey =
    championImageKeys[normalizedName] ??
    championName.replace(/[^a-zA-Z0-9]/g, '')

  return `https://ddragon.leagueoflegends.com/cdn/${DATA_DRAGON_VERSION}/img/champion/${imageKey}.png`
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
