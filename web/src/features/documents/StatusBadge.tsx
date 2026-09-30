import type { TranslationStatus } from '../../api/types'

const STYLES: Record<TranslationStatus, string> = {
    Pending: 'bg-amber-100 text-amber-700',
    Translated: 'bg-emerald-100 text-emerald-700',
    Failed: 'bg-red-100 text-red-700',
}

const LABELS: Record<TranslationStatus, string> = {
    Pending: 'Traduciendo…',
    Translated: 'Traducido',
    Failed: 'Error al traducir',
}

export default function StatusBadge({ status }: { status: TranslationStatus }) {
    return (
        <span className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${STYLES[status]}`}>
            {LABELS[status]}
        </span>
    )
}
