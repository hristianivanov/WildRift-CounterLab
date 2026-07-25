const DATA_DRAGON_VERSION = '14.24.1'

const championImageKeys: Record<string, string> = {
  'dr. mundo': 'DrMundo',
  "kai'sa": 'Kaisa',
  "kha'zix": 'Khazix',
  'nunu & willump': 'Nunu',
  wukong: 'MonkeyKing',
  'master yi': 'MasterYi',
  'jarvan iv': 'JarvanIV',
}

export function getChampionImageUrl(championName: string): string {
  const normalizedName = championName.trim().toLowerCase()
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
