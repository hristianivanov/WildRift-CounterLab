const configuredValue = import.meta.env.VITE_AI_ENABLED?.trim().toLowerCase()

export const aiEnabled = configuredValue
  ? configuredValue === 'true'
  : import.meta.env.DEV
