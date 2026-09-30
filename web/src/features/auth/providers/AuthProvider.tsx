import { useState, type ReactNode } from 'react'
import { authApi } from '../api/authApi'
import { clearStoredToken, getStoredToken, setStoredToken } from '../../../api/authToken'
import type { AuthUser, LoginPayload, RegisterPayload } from '../types/auth.types'
import { AuthContext, type AuthContextValue } from './AuthContext'

const USER_STORAGE_KEY = 'braillesuite_user'

function readStoredUser(): AuthUser | null {
    if (!getStoredToken()) return null
    const stored = localStorage.getItem(USER_STORAGE_KEY)
    return stored ? (JSON.parse(stored) as AuthUser) : null
}

export function AuthProvider({ children }: { children: ReactNode }) {
    // Lazy initializer reads localStorage synchronously on first render —
    // no effect+setState round trip, so there's no extra render/flash.
    const [user, setUser] = useState<AuthUser | null>(readStoredUser)

    async function login(payload: LoginPayload) {
        const response = await authApi.login(payload)
        const authUser: AuthUser = { userId: response.userId, username: response.username }

        setStoredToken(response.token)
        localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(authUser))
        setUser(authUser)
    }

    async function register(payload: RegisterPayload) {
        await authApi.register(payload)
    }

    function logout() {
        clearStoredToken()
        localStorage.removeItem(USER_STORAGE_KEY)
        setUser(null)
    }

    const value: AuthContextValue = {
        user,
        isAuthenticated: user !== null,
        isLoading: false,
        login,
        register,
        logout,
    }

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
