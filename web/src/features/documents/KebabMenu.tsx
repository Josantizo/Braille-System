import { MoreVertical, Pencil, Trash2 } from 'lucide-react'
import { useState } from 'react'

interface KebabMenuProps {
    onRename: () => void
    onDelete: () => void
}

// Small reusable dropdown for the rename/delete actions on folder and
// document rows, following the same overlay + absolute-panel pattern as the
// user menu in Topbar.tsx.
export default function KebabMenu({ onRename, onDelete }: KebabMenuProps) {
    const [open, setOpen] = useState(false)

    return (
        <div className="relative">
            <button
                type="button"
                onClick={(event) => {
                    event.stopPropagation()
                    setOpen((prev) => !prev)
                }}
                className="rounded-md p-1.5 text-muted hover:bg-black/5 hover:text-ink"
                aria-label="Más opciones"
            >
                <MoreVertical size={16} />
            </button>

            {open && (
                <>
                    <div className="fixed inset-0 z-30" onClick={() => setOpen(false)} />
                    <div className="absolute right-0 z-40 mt-1 w-40 rounded-lg border border-black/10 bg-white py-1 shadow-lg">
                        <button
                            type="button"
                            onClick={(event) => {
                                event.stopPropagation()
                                setOpen(false)
                                onRename()
                            }}
                            className="flex w-full items-center gap-2 px-3 py-2 text-left text-sm text-ink hover:bg-black/5"
                        >
                            <Pencil size={14} />
                            Renombrar
                        </button>
                        <button
                            type="button"
                            onClick={(event) => {
                                event.stopPropagation()
                                setOpen(false)
                                onDelete()
                            }}
                            className="flex w-full items-center gap-2 px-3 py-2 text-left text-sm text-red-600 hover:bg-red-50"
                        >
                            <Trash2 size={14} />
                            Eliminar
                        </button>
                    </div>
                </>
            )}
        </div>
    )
}
