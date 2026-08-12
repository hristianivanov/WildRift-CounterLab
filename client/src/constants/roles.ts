export const ROLES = ['Baron', 'Jungle', 'Mid', 'Dragon', 'Support'] as const
export type Role = (typeof ROLES)[number]
