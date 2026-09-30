import { BookOpen } from 'lucide-react'
import { NavLink } from 'react-router-dom'

const NAV_ITEMS = [{ to: '/documentos', label: 'Mis libros', icon: BookOpen }]

export default function Sidebar() {
    return (
        <aside className="hidden w-56 shrink-0 flex-col bg-ink text-ink-text md:flex">
            <div className="flex h-16 items-center px-5 text-lg font-semibold">Braille Suite</div>
            <nav className="flex flex-col gap-1 px-3">
                {NAV_ITEMS.map(({ to, label, icon: Icon }) => (
                    <NavLink
                        key={to}
                        to={to}
                        className={({ isActive }) =>
                            `flex items-center gap-3 rounded-lg px-3 py-2 text-sm transition-colors ${
                                isActive ? 'bg-accent/15 text-accent' : 'text-ink-text/80 hover:bg-ink-soft'
                            }`
                        }
                    >
                        <Icon size={18} />
                        {label}
                    </NavLink>
                ))}
            </nav>
        </aside>
    )
}
