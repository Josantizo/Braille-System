import { X } from 'lucide-react'
import type { ReactNode } from 'react'

interface ModalProps {
    title: string
    onClose: () => void
    children: ReactNode
}

export default function Modal({ title, onClose, children }: ModalProps) {
    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
            <div className="w-full max-w-sm rounded-lg border border-black/10 bg-white p-5 shadow-lg">
                <div className="mb-4 flex items-center justify-between">
                    <h2 className="text-sm font-semibold text-ink">{title}</h2>
                    <button
                        type="button"
                        onClick={onClose}
                        className="rounded-md p-1 text-muted hover:bg-black/5 hover:text-ink"
                        aria-label="Cerrar"
                    >
                        <X size={16} />
                    </button>
                </div>
                {children}
            </div>
        </div>
    )
}
