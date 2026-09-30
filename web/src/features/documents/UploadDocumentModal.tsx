import { useState, type FormEvent } from 'react'
import Modal from './Modal'

interface UploadDocumentModalProps {
    isSubmitting: boolean
    errorMessage: string | null
    onSubmit: (input: { file: File; name?: string }) => void
    onClose: () => void
}

const ACCEPTED_EXTENSIONS = '.txt,.pdf,.docx'

export default function UploadDocumentModal({
    isSubmitting,
    errorMessage,
    onSubmit,
    onClose,
}: UploadDocumentModalProps) {
    const [file, setFile] = useState<File | null>(null)
    const [name, setName] = useState('')

    function handleSubmit(event: FormEvent) {
        event.preventDefault()
        if (!file) return
        onSubmit({ file, name: name.trim() || undefined })
    }

    return (
        <Modal title="Subir libro" onClose={onClose}>
            <form onSubmit={handleSubmit}>
                <label className="mb-3 block">
                    <span className="mb-1 block text-xs font-medium text-muted">Archivo (.txt, .pdf, .docx)</span>
                    <input
                        type="file"
                        autoFocus
                        accept={ACCEPTED_EXTENSIONS}
                        onChange={(event) => setFile(event.target.files?.[0] ?? null)}
                        className="w-full rounded-lg border border-black/10 bg-white px-3 py-2 text-sm text-ink file:mr-3 file:rounded-md file:border-0 file:bg-ink/5 file:px-2 file:py-1 file:text-xs file:text-ink focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
                    />
                </label>

                <label className="block">
                    <span className="mb-1 block text-xs font-medium text-muted">Nombre (opcional)</span>
                    <input
                        type="text"
                        value={name}
                        onChange={(event) => setName(event.target.value)}
                        placeholder={file?.name ?? 'Nombre del libro'}
                        className="w-full rounded-lg border border-black/10 bg-white px-3 py-2 text-sm text-ink placeholder:text-muted focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
                    />
                </label>

                {errorMessage && <p className="mt-2 text-sm text-red-600">{errorMessage}</p>}

                <p className="mt-3 text-xs text-muted">
                    Se traduce a braille automáticamente al terminar de subirse.
                </p>

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
                        disabled={isSubmitting || !file}
                        className="rounded-lg bg-accent px-3 py-1.5 text-sm font-medium text-white hover:bg-accent/90 disabled:opacity-50"
                    >
                        {isSubmitting ? 'Subiendo…' : 'Subir'}
                    </button>
                </div>
            </form>
        </Modal>
    )
}
