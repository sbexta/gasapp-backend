import { Outlet, Navigate } from '@tanstack/react-router'
import { Sidebar } from './Sidebar'
import { useAuthStore } from '@/store/auth'

export function AppLayout() {
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated())

  if (!isAuthenticated) return <Navigate to="/login" />

  return (
    <div className="flex h-screen bg-gray-50">
      <Sidebar />
      <main className="flex-1 overflow-y-auto">
        <Outlet />
      </main>
    </div>
  )
}
