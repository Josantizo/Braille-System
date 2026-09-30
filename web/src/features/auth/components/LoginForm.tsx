import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { ApiError } from '../../../api/client'
import { useAuth } from '../hooks/useAuth'

export default function LoginForm() {
    const { login } = useAuth()
    const navigate = useNavigate()
    const [usernameOrEmail, setUsernameOrEmail] = useState('')
    const [password, setPassword] = useState('')
    const [error, setError] = useState<string | null>(null)
    const [isSubmitting, setIsSubmitting] = useState(false)

    async function handleSubmit(event: FormEvent) {
        event.preventDefault()
        setError(null)
        setIsSubmitting(true)
        try {
            await login({ usernameOrEmail, password })
            navigate('/documentos', { replace: true })
        } catch (err) {
            setError(err instanceof ApiError ? err.message : 'Ocurrió un error inesperado.')
        } finally {
            setIsSubmitting(false)
        }
    }

    return (
        <div className="flex min-h-screen items-center justify-center bg-paper px-4">
            <div className="w-full max-w-sm rounded-lg border border-black/10 bg-white p-6 shadow-lg">
                <h1 className="mb-1 text-lg font-semibold text-ink">Iniciar sesión</h1>
                <p className="mb-5 text-sm text-muted">Sistema de Braille</p>

                <form onSubmit={handleSubmit} className="flex flex-col gap-3">
                    <label className="block">
                        <span className="mb-1 block text-xs font-medium text-muted">Usuario o correo</span>
                        <input
                            type="text"
                            required
                            autoFocus
                            value={usernameOrEmail}
                            onChange={(event) => setUsernameOrEmail(event.target.value)}
                            className="w-full rounded-lg border border-black/10 bg-white px-3 py-2 text-sm text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
                        />
                    </label>

                    <label className="block">
                        <span className="mb-1 block text-xs font-medium text-muted">Contraseña</span>
                        <input
                            type="password"
                            required
                            value={password}
                            onChange={(event) => setPassword(event.target.value)}
                            className="w-full rounded-lg border border-black/10 bg-white px-3 py-2 text-sm text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
                        />
                    </label>

                    {error && <p className="text-sm text-red-600">{error}</p>}

                    <button
                        type="submit"
                        disabled={isSubmitting}
                        className="mt-2 rounded-lg bg-accent px-3 py-2 text-sm font-medium text-white hover:bg-accent/90 disabled:opacity-50"
                    >
                        {isSubmitting ? 'Ingresando…' : 'Ingresar'}
                    </button>
                </form>

                <p className="mt-4 text-center text-sm text-muted">
                    ¿No tienes cuenta?{' '}
                    <Link to="/register" className="font-medium text-accent hover:underline">
                        Regístrate
                    </Link>
                </p>
            </div>
        </div>
    )
}
