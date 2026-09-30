// Shared between the API client (reads the token to attach it to requests)
// and the auth feature (writes it on login/logout). Keeping the storage key
// in one place avoids the two getting out of sync.
const TOKEN_STORAGE_KEY = 'braillesuite_token'

export function getStoredToken(): string | null {
    return localStorage.getItem(TOKEN_STORAGE_KEY)
}

export function setStoredToken(token: string): void {
    localStorage.setItem(TOKEN_STORAGE_KEY, token)
}

export function clearStoredToken(): void {
    localStorage.removeItem(TOKEN_STORAGE_KEY)
}
