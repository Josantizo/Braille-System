export interface RegisterPayload {
    username: string
    email: string
    password: string
}

export interface LoginPayload {
    usernameOrEmail: string
    password: string
}

export interface RegisterResponse {
    userId: number
    username: string
    email: string
}

export interface LoginResponse {
    token: string
    userId: number
    username: string
}

export interface AuthUser {
    userId: number
    username: string
}
