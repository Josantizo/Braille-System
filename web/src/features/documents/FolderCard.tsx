import { Folder } from 'lucide-react'
import type { FolderSummaryDto } from '../../api/types'
import KebabMenu from './KebabMenu'

interface FolderCardProps {
    folder: FolderSummaryDto
    onOpen: () => void
    onRename: () => void
    onDelete: () => void
}

export default function FolderCard({ folder, onOpen, onRename, onDelete }: FolderCardProps) {
    return (
        <button
            type="button"
            onClick={onOpen}
            className="flex items-center gap-3 rounded-lg border border-black/10 bg-white p-3 text-left transition hover:border-accent/40 hover:bg-black/5"
        >
            <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-accent/15 text-accent">
                <Folder size={20} />
            </span>
            <span className="min-w-0 flex-1 truncate text-sm font-medium text-ink">{folder.name}</span>
            <KebabMenu onRename={onRename} onDelete={onDelete} />
        </button>
    )
}
