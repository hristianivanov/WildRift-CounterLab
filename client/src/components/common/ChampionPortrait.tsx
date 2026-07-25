import { useEffect, useState } from 'react'

import { getChampionImageUrl, getChampionInitials } from '../../utils/championImages'

interface ChampionPortraitProps {
  championName: string
  className?: string
  imageClassName?: string
}

export default function ChampionPortrait({
  championName,
  className = '',
  imageClassName = '',
}: ChampionPortraitProps) {
  const [imageFailed, setImageFailed] = useState(false)

  useEffect(() => {
    setImageFailed(false)
  }, [championName])

  return (
    <span
      className={`relative grid shrink-0 place-items-center overflow-hidden bg-gradient-to-br from-cyan-300/25 to-violet-400/20 font-black text-cyan-100 ${className}`}
    >
      {!imageFailed ? (
        <img
          src={getChampionImageUrl(championName)}
          alt={`${championName} portrait`}
          loading="lazy"
          onError={() => setImageFailed(true)}
          className={`size-full object-cover ${imageClassName}`}
        />
      ) : (
        <span aria-label={`${championName} portrait fallback`}>
          {getChampionInitials(championName)}
        </span>
      )}
    </span>
  )
}
