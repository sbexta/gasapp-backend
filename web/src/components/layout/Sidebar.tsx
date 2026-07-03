import { useState } from 'react'
import { Link, useLocation } from '@tanstack/react-router'
import {
  LayoutDashboard, Users, Building2, ClipboardList, LogOut, Flame, MapPin, ShieldCheck, FileText, Search, Lock, ListChecks, BarChart2, KeyRound,
} from 'lucide-react'
import { useMutation } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { cn } from '@/lib/utils'
import { useAuthStore } from '@/store/auth'
import { api } from '@/lib/api'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'

const navItems = [
  { to: '/dashboard', icon: LayoutDashboard, label: 'Dashboard', roles: ['Admin', 'Supervisor', 'Technician'] },
  { to: '/clients', icon: Building2, label: 'Clientes', roles: ['Admin', 'Supervisor'] },
  { to: '/contracts', icon: FileText, label: 'Contratos', roles: ['Admin', 'Supervisor'] },
  { to: '/locations', icon: MapPin, label: 'Sedes', roles: ['Admin', 'Supervisor'] },
  { to: '/inspection-types', icon: ShieldCheck, label: 'Tipos de inspección', roles: ['Admin', 'Supervisor'] },
  { to: '/checklist-templates', icon: ListChecks, label: 'Checklists', roles: ['Admin', 'Supervisor'] },
  { to: '/work-orders', icon: ClipboardList, label: 'Órdenes de trabajo', roles: ['Admin', 'Supervisor'] },
  { to: '/inspections', icon: Search, label: 'Inspecciones', roles: ['Admin', 'Supervisor'] },
  { to: '/reports', icon: BarChart2, label: 'Reportes', roles: ['Admin', 'Supervisor'] },
  { to: '/users', icon: Users, label: 'Usuarios', roles: ['Admin'] },
  { to: '/permissions', icon: Lock, label: 'Permisos', roles: ['Admin'] },
]

const changePasswordSchema = z.object({
  currentPassword: z.string().min(1, 'Requerido'),
  newPassword: z.string()
    .min(8, 'Mínimo 8 caracteres')
    .regex(/[A-Z]/, 'Debe tener al menos una mayúscula')
    .regex(/[0-9]/, 'Debe tener al menos un número'),
  confirm: z.string(),
}).refine((v) => v.newPassword === v.confirm, { message: 'Las contraseñas no coinciden', path: ['confirm'] })

type ChangePasswordForm = z.infer<typeof changePasswordSchema>

export function Sidebar() {
  const { pathname } = useLocation()
  const { user, clearAuth } = useAuthStore()
  const [showChangePassword, setShowChangePassword] = useState(false)

  const visible = navItems.filter((item) => user?.role && item.roles.includes(user.role))

  const form = useForm<ChangePasswordForm>({ resolver: zodResolver(changePasswordSchema) })

  const changeMutation = useMutation({
    mutationFn: (values: ChangePasswordForm) =>
      api.patch('/users/me/change-password', {
        currentPassword: values.currentPassword,
        newPassword: values.newPassword,
      }),
    onSuccess: () => {
      setShowChangePassword(false)
      form.reset()
    },
  })

  return (
    <>
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
            onClick={() => setShowChangePassword(true)}
            className="flex w-full items-center gap-3 rounded-md px-3 py-2 text-sm font-medium text-gray-600 hover:bg-gray-100 hover:text-gray-900"
          >
            <KeyRound className="h-4 w-4" />
            Cambiar contraseña
          </button>
          <button
            onClick={clearAuth}
            className="flex w-full items-center gap-3 rounded-md px-3 py-2 text-sm font-medium text-gray-600 hover:bg-gray-100 hover:text-gray-900"
          >
            <LogOut className="h-4 w-4" />
            Cerrar sesión
          </button>
        </div>
      </aside>

      {/* Modal cambiar contraseña propia */}
      {showChangePassword && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="w-full max-w-md rounded-xl bg-white p-6 shadow-xl">
            <h2 className="mb-5 text-lg font-semibold text-gray-900">Cambiar contraseña</h2>
            <form onSubmit={form.handleSubmit((v) => changeMutation.mutate(v))} className="space-y-4">
              <div>
                <Label htmlFor="currentPassword">Contraseña actual</Label>
                <Input id="currentPassword" type="password" {...form.register('currentPassword')} />
                {form.formState.errors.currentPassword && (
                  <p className="mt-1 text-xs text-red-500">{form.formState.errors.currentPassword.message}</p>
                )}
              </div>
              <div>
                <Label htmlFor="newPassword">Nueva contraseña</Label>
                <Input id="newPassword" type="password" {...form.register('newPassword')} />
                {form.formState.errors.newPassword && (
                  <p className="mt-1 text-xs text-red-500">{form.formState.errors.newPassword.message}</p>
                )}
                <p className="mt-1 text-xs text-gray-400">Mínimo 8 caracteres, una mayúscula y un número</p>
              </div>
              <div>
                <Label htmlFor="confirm">Confirmar nueva contraseña</Label>
                <Input id="confirm" type="password" {...form.register('confirm')} />
                {form.formState.errors.confirm && (
                  <p className="mt-1 text-xs text-red-500">{form.formState.errors.confirm.message}</p>
                )}
              </div>
              {changeMutation.isError && (
                <p className="text-sm text-red-500">Error: contraseña actual incorrecta o contraseña inválida.</p>
              )}
              {changeMutation.isSuccess && (
                <p className="text-sm text-green-600">Contraseña actualizada correctamente.</p>
              )}
              <div className="flex justify-end gap-3 pt-2">
                <Button type="button" variant="outline" onClick={() => { setShowChangePassword(false); form.reset() }}>
                  Cancelar
                </Button>
                <Button type="submit" disabled={changeMutation.isPending}>
                  {changeMutation.isPending ? 'Guardando...' : 'Cambiar contraseña'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  )
}
