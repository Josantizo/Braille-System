import { ChevronRight, Home } from 'lucide-react'
import type { BreadcrumbItemDto } from '../../api/types'

interface BreadcrumbProps {
    items: BreadcrumbItemDto[]
    onNavigate: (folderId: string | null) => void
}

export default function Breadcrumb({ items, onNavigate }: BreadcrumbProps) {
    return (
        <nav className="flex flex-wrap items-center gap-1 text-sm text-muted">
            <button
                type="button"
                onClick={() => onNavigate(null)}
                className="flex items-center gap-1 rounded px-1.5 py-1 hover:bg-black/5 hover:text-ink"
            >
                <Home size={14} />
                Inicio
            </button>

            {items.map((item) => (
                <span key={item.id} className="flex items-center gap-1">
                    <ChevronRight size={14} />
                    <button
                        type="button"
                        onClick={() => onNavigate(item.id)}
                        className="rounded px-1.5 py-1 hover:bg-black/5 hover:text-ink"
                    >
                        {item.name}
                    </button>
                </span>
            ))}
        </nav>
    )
}
