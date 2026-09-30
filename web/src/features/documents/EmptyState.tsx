import { FolderOpen } from 'lucide-react'

export default function EmptyState() {
    return (
        <div className="flex flex-col items-center justify-center gap-3 rounded-lg border border-dashed border-black/10 bg-white py-16 text-center">
            <FolderOpen size={32} className="text-muted" />
            <p className="text-sm text-muted">Esta carpeta está vacía.</p>
        </div>
    )
}
