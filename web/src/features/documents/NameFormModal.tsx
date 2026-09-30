import { useState, type FormEvent } from 'react'
import Modal from './Modal'

interface NameFormModalProps {
    title: string
    initialName?: string
    submitLabel: string
    isSubmitting: boolean
    errorMessage: string | null
    onSubmit: (name: string) => void
    onClose: () => void
}

// Shared by "create folder" and "rename folder/document" — both are just a
// single name field with a submit/cancel action.
export default function NameFormModal({
    title,
    initialName = '',
    submitLabel,
    isSubmitting,
    errorMessage,
    onSubmit,
    onClose,
}: NameFormModalProps) {
    const [name, setName] = useState(initialName)

    function handleSubmit(event: FormEvent) {
        event.preventDefault()
        const trimmed = name.trim()
        if (trimmed) onSubmit(trimmed)
    }

    return (
        <Modal title={title} onClose={onClose}>
            <form onSubmit={handleSubmit}>
                <input
                    type="text"
                    autoFocus
                    value={name}
                    onChange={(event) => setName(event.target.value)}
                    placeholder="Nombre"
                    className="w-full rounded-lg border border-black/10 bg-white px-3 py-2 text-sm text-ink placeholder:text-muted focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
                />

                {errorMessage && <p className="mt-2 text-sm text-red-600">{errorMessage}</p>}

                <div className="mt-4 flex justify-end gap-2">
                    <button
                        type="button"
                        onClick={onClose}
                        className="rounded-lg px-3 py-1.5 text-sm text-ink hover:bg-black/5"
                    >
                        Cancelar
                    </button>
                    <button
                        type="submit"
                        disabled={isSubmitting || !name.trim()}
                        className="rounded-lg bg-accent px-3 py-1.5 text-sm font-medium text-white hover:bg-accent/90 disabled:opacity-50"
                    >
                        {isSubmitting ? 'Guardando…' : submitLabel}
                    </button>
                </div>
            </form>
        </Modal>
    )
}
