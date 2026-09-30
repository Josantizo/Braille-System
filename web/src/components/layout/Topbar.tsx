import { ChevronDown } from 'lucide-react'
import { useState } from 'react'
import { LogoutButton, useAuth } from '../../features/auth'

export default function Topbar() {
    const { user } = useAuth()
    const [menuOpen, setMenuOpen] = useState(false)
    const displayName = user?.username ?? 'Usuario'

    return (
        <header className="sticky top-0 z-30 flex h-16 items-center justify-end gap-4 border-b border-black/5 bg-paper/95 px-4 backdrop-blur">
            <div className="relative">
                <button
                    type="button"
                    onClick={() => setMenuOpen((prev) => !prev)}
                    className="flex items-center gap-2 rounded-lg px-2 py-1.5 hover:bg-black/5"
                >
                    <span className="flex h-9 w-9 items-center justify-center rounded-full bg-ink text-sm font-medium text-ink-text">
                        {displayName.slice(0, 2).toUpperCase()}
                    </span>
                    <span className="hidden text-sm font-medium text-ink sm:block">{displayName}</span>
                    <ChevronDown size={16} className="hidden text-muted sm:block" />
                </button>

                {menuOpen && (
                    <>
                        <div className="fixed inset-0 z-30" onClick={() => setMenuOpen(false)} />
                        <div className="absolute right-0 z-40 mt-2 w-48 rounded-lg border border-black/10 bg-white py-2 shadow-lg">
                            <LogoutButton />
                        </div>
                    </>
                )}
            </div>
        </header>
    )
}
