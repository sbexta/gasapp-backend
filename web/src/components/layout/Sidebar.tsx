import { Link, useLocation } from '@tanstack/react-router'
import {
  LayoutDashboard, Users, Building2, ClipboardList, LogOut, Flame, MapPin, ShieldCheck, FileText, Search, Lock,
} from 'lucide-react'
import { cn } from '@/lib/utils'
import { useAuthStore } from '@/store/auth'

const navItems = [
  { to: '/dashboard', icon: LayoutDashboard, label: 'Dashboard', roles: ['Admin', 'Supervisor', 'Technician'] },
  { to: '/clients', icon: Building2, label: 'Clientes', roles: ['Admin', 'Supervisor'] },
  { to: '/contracts', icon: FileText, label: 'Contratos', roles: ['Admin', 'Supervisor'] },
  { to: '/locations', icon: MapPin, label: 'Sedes', roles: ['Admin', 'Supervisor'] },
  { to: '/inspection-types', icon: ShieldCheck, label: 'Tipos de inspección', roles: ['Admin', 'Supervisor'] },
  { to: '/work-orders', icon: ClipboardList, label: 'Órdenes de trabajo', roles: ['Admin', 'Supervisor'] },
  { to: '/inspections', icon: Search, label: 'Inspecciones', roles: ['Admin', 'Supervisor'] },
  { to: '/users', icon: Users, label: 'Usuarios', roles: ['Admin'] },
  { to: '/permissions', icon: Lock, label: 'Permisos', roles: ['Admin'] },
]

export function Sidebar() {
  const { pathname } = useLocation()
  const { user, clearAuth } = useAuthStore()

  const visible = navItems.filter((item) => user?.role && item.roles.includes(user.role))

  return (
    <aside className="flex h-screen w-64 flex-col border-r border-gray-200 bg-white">
      <div className="flex h-16 items-center gap-2 border-b border-gray-200 px-6">
        <Flame className="h-6 w-6 text-blue-600" />
        <span className="text-lg font-bold text-gray-900">GasApp</span>
      </div>

      <nav className="flex-1 overflow-y-auto p-4">
        <ul className="space-y-1">
          {visible.map(({ to, icon: Icon, label }) => (
            <li key={to}>
              <Link
                to={to}
                className={cn(
                  'flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors',
                  pathname.startsWith(to)
                    ? 'bg-blue-50 text-blue-700'
                    : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'
                )}
              >
                <Icon className="h-4 w-4" />
                {label}
              </Link>
            </li>
          ))}
        </ul>
      </nav>

      <div className="border-t border-gray-200 p-4">
        <div className="mb-3 px-3">
          <p className="text-sm font-medium text-gray-900">{user?.fullName}</p>
          <p className="text-xs text-gray-500">{user?.role}</p>
        </div>
        <button
          onClick={clearAuth}
          className="flex w-full items-center gap-3 rounded-md px-3 py-2 text-sm font-medium text-gray-600 hover:bg-gray-100 hover:text-gray-900"
        >
          <LogOut className="h-4 w-4" />
          Cerrar sesión
        </button>
      </div>
    </aside>
  )
}
