import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { ApiError } from '../../../api/client'
import { useAuth } from '../hooks/useAuth'

export default function RegisterForm() {
    const { register } = useAuth()
    const navigate = useNavigate()
    const [username, setUsername] = useState('')
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [confirmPassword, setConfirmPassword] = useState('')
    const [error, setError] = useState<string | null>(null)
    const [isSubmitting, setIsSubmitting] = useState(false)

    async function handleSubmit(event: FormEvent) {
        event.preventDefault()
        setError(null)

        if (password !== confirmPassword) {
            setError('Las contraseñas no coinciden.')
            return
        }

        setIsSubmitting(true)
        try {
            await register({ username, email, password })
            navigate('/login', { replace: true })
        } catch (err) {
            setError(err instanceof ApiError ? err.message : 'Ocurrió un error inesperado.')
        } finally {
            setIsSubmitting(false)
        }
    }

    return (
        <div className="flex min-h-screen items-center justify-center bg-paper px-4">
            <div className="w-full max-w-sm rounded-lg border border-black/10 bg-white p-6 shadow-lg">
                <h1 className="mb-1 text-lg font-semibold text-ink">Crear cuenta</h1>
                <p className="mb-5 text-sm text-muted">Sistema de Braille</p>

                <form onSubmit={handleSubmit} className="flex flex-col gap-3">
                    <label className="block">
                        <span className="mb-1 block text-xs font-medium text-muted">Usuario</span>
                        <input
                            type="text"
                            required
                            autoFocus
                            value={username}
                            onChange={(event) => setUsername(event.target.value)}
                            className="w-full rounded-lg border border-black/10 bg-white px-3 py-2 text-sm text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
                        />
                    </label>

                    <label className="block">
                        <span className="mb-1 block text-xs font-medium text-muted">Correo</span>
                        <input
                            type="email"
                            required
                            value={email}
                            onChange={(event) => setEmail(event.target.value)}
                            className="w-full rounded-lg border border-black/10 bg-white px-3 py-2 text-sm text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
                        />
                    </label>

                    <label className="block">
                        <span className="mb-1 block text-xs font-medium text-muted">Contraseña</span>
                        <input
                            type="password"
                            required
                            minLength={8}
                            value={password}
                            onChange={(event) => setPassword(event.target.value)}
                            className="w-full rounded-lg border border-black/10 bg-white px-3 py-2 text-sm text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
                        />
                    </label>

                    <label className="block">
                        <span className="mb-1 block text-xs font-medium text-muted">Confirmar contraseña</span>
                        <input
                            type="password"
                            required
                            minLength={8}
                            value={confirmPassword}
                            onChange={(event) => setConfirmPassword(event.target.value)}
                            className="w-full rounded-lg border border-black/10 bg-white px-3 py-2 text-sm text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
                        />
                    </label>

                    {error && <p className="text-sm text-red-600">{error}</p>}

                    <button
                        type="submit"
                        disabled={isSubmitting}
                        className="mt-2 rounded-lg bg-accent px-3 py-2 text-sm font-medium text-white hover:bg-accent/90 disabled:opacity-50"
                    >
                        {isSubmitting ? 'Creando cuenta…' : 'Registrarme'}
                    </button>
                </form>

                <p className="mt-4 text-center text-sm text-muted">
                    ¿Ya tienes cuenta?{' '}
                    <Link to="/login" className="font-medium text-accent hover:underline">
                        Inicia sesión
                    </Link>
                </p>
            </div>
        </div>
    )
}
