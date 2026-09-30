import Modal from './Modal'

interface ConfirmDeleteDialogProps {
    itemName: string
    isSubmitting: boolean
    errorMessage: string | null
    onConfirm: () => void
    onClose: () => void
}

export default function ConfirmDeleteDialog({
    itemName,
    isSubmitting,
    errorMessage,
    onConfirm,
    onClose,
}: ConfirmDeleteDialogProps) {
    return (
        <Modal title="Eliminar" onClose={onClose}>
            <p className="text-sm text-ink">
                ¿Seguro que quieres eliminar <span className="font-medium">{itemName}</span>? Esta acción no se
                puede deshacer.
            </p>

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
                    type="button"
                    onClick={onConfirm}
                    disabled={isSubmitting}
                    className="rounded-lg bg-red-600 px-3 py-1.5 text-sm font-medium text-white hover:bg-red-700 disabled:opacity-50"
                >
                    {isSubmitting ? 'Eliminando…' : 'Eliminar'}
                </button>
            </div>
        </Modal>
    )
}
