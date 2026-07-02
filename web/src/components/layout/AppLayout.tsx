import { Outlet, Navigate } from '@tanstack/react-router'
import { useState, useRef, useEffect } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Bell } from 'lucide-react'
import { Sidebar } from './Sidebar'
import { useAuthStore } from '@/store/auth'
import { api } from '@/lib/api'

interface NotifDto { id: string; title: string; body: string; isRead: boolean; createdAt: string; type: string; referenceId: string | null }
interface NotifResult { items: NotifDto[]; total: number; unreadCount: number }

function NotificationBell() {
  const [open, setOpen] = useState(false)
  const ref = useRef<HTMLDivElement>(null)
  const qc = useQueryClient()

  const { data } = useQuery({
    queryKey: ['notifications'],
    queryFn: () => api.get<NotifResult>('/notifications?pageSize=10').then(r => r.data),
    refetchInterval: 30_000,
  })

  const markRead = useMutation({
    mutationFn: (id: string) => api.put(`/notifications/${id}/read`),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['notifications'] }),
  })

  useEffect(() => {
    function handleClick(e: MouseEvent) {
      if (ref.current && !ref.current.contains(e.target as Node)) setOpen(false)
    }
    document.addEventListener('mousedown', handleClick)
    return () => document.removeEventListener('mousedown', handleClick)
  }, [])

  const unread = data?.unreadCount ?? 0

  return (
    <div ref={ref} className="relative">
      <button
        onClick={() => setOpen(o => !o)}
        className="relative flex h-9 w-9 items-center justify-center rounded-lg text-gray-500 hover:bg-gray-100"
      >
        <Bell className="h-5 w-5" />
        {unread > 0 && (
          <span className="absolute -right-0.5 -top-0.5 flex h-4 w-4 items-center justify-center rounded-full bg-red-500 text-[10px] font-bold text-white">
            {unread > 9 ? '9+' : unread}
          </span>
        )}
      </button>

      {open && (
        <div className="absolute right-0 top-11 z-50 w-80 rounded-xl border border-gray-200 bg-white shadow-xl">
          <div className="flex items-center justify-between border-b border-gray-200 px-4 py-3">
            <p className="text-sm font-semibold text-gray-900">Notificaciones</p>
            {unread > 0 && <span className="text-xs text-gray-500">{unread} sin leer</span>}
          </div>
          <ul className="max-h-80 overflow-y-auto">
            {(!data?.items || data.items.length === 0) && (
              <li className="px-4 py-6 text-center text-sm text-gray-400">Sin notificaciones</li>
            )}
            {data?.items.map(n => (
              <li
                key={n.id}
                className={`cursor-pointer border-b border-gray-100 px-4 py-3 hover:bg-gray-50 last:border-0 ${!n.isRead ? 'bg-blue-50/40' : ''}`}
                onClick={() => { if (!n.isRead) markRead.mutate(n.id) }}
              >
                <div className="flex items-start gap-2">
                  {!n.isRead && <span className="mt-1.5 h-2 w-2 shrink-0 rounded-full bg-blue-500" />}
                  <div className={!n.isRead ? '' : 'pl-4'}>
                    <p className="text-sm font-medium text-gray-900">{n.title}</p>
                    <p className="text-xs text-gray-500">{n.body}</p>
                    <p className="mt-1 text-xs text-gray-400">
                      {new Date(n.createdAt).toLocaleString('es-CO')}
                    </p>
                  </div>
                </div>
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  )
}

export function AppLayout() {
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated())

  if (!isAuthenticated) return <Navigate to="/login" />

  return (
    <div className="flex h-screen bg-gray-50">
      <Sidebar />
      <div className="flex flex-1 flex-col overflow-hidden">
        <header className="flex h-12 items-center justify-end border-b border-gray-200 bg-white px-6">
          <NotificationBell />
        </header>
        <main className="flex-1 overflow-y-auto">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
