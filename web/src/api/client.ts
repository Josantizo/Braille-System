import { getStoredToken } from './authToken'

const BASE_URL = (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:5200'

export class ApiError extends Error {
    readonly status: number

    constructor(status: number, message: string) {
        super(message)
        this.name = 'ApiError'
        this.status = status
    }
}

function authHeaders(): Record<string, string> {
    const token = getStoredToken()
    return token ? { Authorization: `Bearer ${token}` } : {}
}

async function parseErrorMessage(response: Response): Promise<string> {
    try {
        const problem = (await response.clone().json()) as { detail?: string; title?: string }
        return problem.detail ?? problem.title ?? response.statusText
    } catch {
        return response.statusText || `Error ${response.status}`
    }
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
    let response: Response
    try {
        response = await fetch(`${BASE_URL}${path}`, {
            ...init,
            headers: {
                ...authHeaders(),
                ...init?.headers,
            },
        })
    } catch {
        throw new ApiError(0, 'No se pudo conectar con el servidor.')
    }

    if (!response.ok) {
        throw new ApiError(response.status, await parseErrorMessage(response))
    }

    if (response.status === 204) {
        return undefined as T
    }

    return (await response.json()) as T
}

export function apiGet<T>(path: string): Promise<T> {
    return request<T>(path)
}

export function apiPost<T>(path: string, body?: unknown): Promise<T> {
    return request<T>(path, {
        method: 'POST',
        headers: body === undefined ? undefined : { 'Content-Type': 'application/json' },
        body: body === undefined ? undefined : JSON.stringify(body),
    })
}

export function apiPostForm<T>(path: string, form: FormData): Promise<T> {
    return request<T>(path, { method: 'POST', body: form })
}

export function apiPut<T>(path: string, body: unknown): Promise<T> {
    return request<T>(path, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
    })
}

export function apiDelete(path: string): Promise<void> {
    return request<void>(path, { method: 'DELETE' })
}

export async function apiDownload(path: string): Promise<Blob> {
    const response = await fetch(`${BASE_URL}${path}`, { headers: authHeaders() })

    if (!response.ok) {
        throw new ApiError(response.status, await parseErrorMessage(response))
    }

    return response.blob()
}
