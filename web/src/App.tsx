import { Navigate, Outlet, Route, Routes } from 'react-router-dom'
import AppLayout from './components/layout/AppLayout'
import DocumentsPage from './features/documents/DocumentsPage'
import { LoginForm, ProtectedRoute, RegisterForm } from './features/auth'

function App() {
    return (
        <Routes>
            {/* Rutas públicas */}
            <Route path="/login" element={<LoginForm />} />
            <Route path="/register" element={<RegisterForm />} />

            {/* Rutas protegidas, envueltas en AppLayout (Sidebar + Topbar) */}
            <Route element={<ProtectedRoute />}>
                <Route
                    element={
                        <AppLayout>
                            <Outlet />
                        </AppLayout>
                    }
                >
                    <Route path="/documentos" element={<DocumentsPage />} />
                </Route>
            </Route>

            {/* Cualquier ruta no reconocida, redirige */}
            <Route path="*" element={<Navigate to="/documentos" replace />} />
        </Routes>
    )
}

export default App
