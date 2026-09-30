import { apiPost } from '../../../api/client'
import type { LoginPayload, LoginResponse, RegisterPayload, RegisterResponse } from '../types/auth.types'

export const authApi = {
    register: (payload: RegisterPayload) => apiPost<RegisterResponse>('/api/auth/register', payload),
    login: (payload: LoginPayload) => apiPost<LoginResponse>('/api/auth/login', payload),
}
