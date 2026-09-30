import { createContext } from 'react'
import type { AuthUser, LoginPayload, RegisterPayload } from '../types/auth.types'

export interface AuthContextValue {
    user: AuthUser | null
    isAuthenticated: boolean
    isLoading: boolean
    login: (payload: LoginPayload) => Promise<void>
    register: (payload: RegisterPayload) => Promise<void>
    logout: () => void
}

// Kept in its own file (not the provider component's) so that file only
// exports the component — mixing a non-component export in there breaks
// Vite/React Fast Refresh for that file.
export const AuthContext = createContext<AuthContextValue | undefined>(undefined)
