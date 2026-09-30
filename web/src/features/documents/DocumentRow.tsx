import { Download, FileText, RotateCw } from 'lucide-react'
import { useState } from 'react'
import * as documentsApi from '../../api/documents'
import type { DocumentSummaryDto } from '../../api/types'
import KebabMenu from './KebabMenu'
import StatusBadge from './StatusBadge'

interface DocumentRowProps {
    doc: DocumentSummaryDto
    onRename: () => void
    onDelete: () => void
    onRetranslate: () => void
    isRetranslating: boolean
}

export default function DocumentRow({ doc, onRename, onDelete, onRetranslate, isRetranslating }: DocumentRowProps) {
    const [downloading, setDownloading] = useState<'original' | 'translated' | null>(null)

    async function handleDownload(kind: 'original' | 'translated') {
        setDownloading(kind)
        try {
            if (kind === 'original') {
                await documentsApi.downloadOriginal(doc.id, doc.originalFileName)
            } else {
                await documentsApi.downloadTranslated(doc.id, `${doc.name}-braille.pdf`)
            }
        } finally {
            setDownloading(null)
        }
    }

    return (
        <div className="flex items-center gap-3 rounded-lg border border-black/10 bg-white p-3">
            <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-ink/5 text-ink">
                <FileText size={20} />
            </span>

            <div className="min-w-0 flex-1">
                <p className="truncate text-sm font-medium text-ink">{doc.name}</p>
                <p className="truncate text-xs text-muted" title={doc.translationError ?? undefined}>
                    {doc.originalFileName}
                </p>
            </div>

            <StatusBadge status={doc.translationStatus} />

            <div className="flex items-center gap-1">
                <button
                    type="button"
                    onClick={() => void handleDownload('original')}
                    disabled={downloading === 'original'}
                    className="rounded-md p-1.5 text-muted hover:bg-black/5 hover:text-ink disabled:opacity-50"
                    title="Descargar original"
                >
                    <Download size={16} />
                </button>
                {doc.hasTranslatedContent && (
                    <button
                        type="button"
                        onClick={() => void handleDownload('translated')}
                        disabled={downloading === 'translated'}
                        className="rounded-md px-2 py-1 text-xs font-medium text-accent hover:bg-accent/10 disabled:opacity-50"
                        title="Descargar PDF en braille"
                    >
                        Braille
                    </button>
                )}
                {doc.translationStatus === 'Failed' && (
                    <button
                        type="button"
                        onClick={onRetranslate}
                        disabled={isRetranslating}
                        className="rounded-md p-1.5 text-muted hover:bg-black/5 hover:text-ink disabled:opacity-50"
                        title={doc.translationError ?? 'Reintentar traducción'}
                    >
                        <RotateCw size={16} className={isRetranslating ? 'animate-spin' : undefined} />
                    </button>
                )}
            </div>

            <KebabMenu onRename={onRename} onDelete={onDelete} />
        </div>
    )
}
