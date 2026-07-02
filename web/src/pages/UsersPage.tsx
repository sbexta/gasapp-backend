import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { api } from '@/lib/api'
import type { PagedResult, UserDto } from '@/types/api'

const roleLabel: Record<string, string> = {
  Admin: 'Admin', Supervisor: 'Supervisor', Technician: 'Técnico', Client: 'Cliente',
}
const roleVariant: Record<string, 'default' | 'success' | 'warning' | 'destructive' | 'secondary'> = {
  Admin: 'destructive', Supervisor: 'warning', Technician: 'default', Client: 'secondary',
}

const createSchema = z.object({
  email: z.string().email('Correo inválido'),
  password: z.string()
    .min(8, 'Mínimo 8 caracteres')
    .regex(/[A-Z]/, 'Debe tener al menos una mayúscula')
    .regex(/[0-9]/, 'Debe tener al menos un número'),
  firstName: z.string().min(1, 'Requerido'),
  lastName: z.string().min(1, 'Requerido'),
  role: z.enum(['Admin', 'Supervisor', 'Technician', 'Client']),
  phone: z.string().optional(),
})
type CreateForm = z.infer<typeof createSchema>

export function UsersPage() {
  const qc = useQueryClient()
  const [search, setSearch] = useState('')
  const [roleFilter, setRoleFilter] = useState('')
  const [statusFilter, setStatusFilter] = useState('')
  const [showCreate, setShowCreate] = useState(false)

  const { data, isLoading } = useQuery({
    queryKey: ['users', search, roleFilter, statusFilter],
    queryFn: () => {
      const params = new URLSearchParams({ pageSize: '50' })
      if (search) params.set('search', search)
      if (roleFilter) params.set('role', roleFilter)
      if (statusFilter !== '') params.set('isActive', statusFilter)
      return api.get<PagedResult<UserDto>>(`/users?${params}`).then((r) => r.data)
    },
  })

  const form = useForm<CreateForm>({
    resolver: zodResolver(createSchema),
    defaultValues: { role: 'Technician' },
  })

  const createMutation = useMutation({
    mutationFn: (values: CreateForm) => api.post('/users', values),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['users'] })
      setShowCreate(false)
      form.reset()
    },
  })

  const toggleMutation = useMutation({
    mutationFn: ({ id, activate }: { id: string; activate: boolean }) =>
      api.patch(`/users/${id}/${activate ? 'activate' : 'deactivate'}`),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['users'] }),
  })

  return (
    <div className="p-8">
      <div className="mb-6 flex items-start justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Usuarios</h1>
          <p className="text-sm text-gray-500">{data?.totalCount ?? 0} usuarios registrados</p>
        </div>
        <Button onClick={() => setShowCreate(true)}>+ Nuevo usuario</Button>
      </div>

      {/* Filtros */}
      <div className="mb-4 flex items-center gap-3">
        <Input
          placeholder="Buscar por nombre o correo..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="max-w-xs"
        />
        <select
          value={roleFilter}
          onChange={(e) => setRoleFilter(e.target.value)}
          className="h-9 rounded-md border border-gray-300 bg-white px-3 text-sm"
        >
          <option value="">Todos los roles</option>
          {Object.entries(roleLabel).map(([val, label]) => (
            <option key={val} value={val}>{label}</option>
          ))}
        </select>
        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
          className="h-9 rounded-md border border-gray-300 bg-white px-3 text-sm"
        >
          <option value="">Todos los estados</option>
          <option value="true">Activos</option>
          <option value="false">Inactivos</option>
        </select>
      </div>

      <div className="rounded-lg border border-gray-200 bg-white">
        <table className="w-full text-sm">
          <thead className="border-b border-gray-200 bg-gray-50">
            <tr>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Nombre</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Correo</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Rol</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Estado</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Acciones</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {isLoading ? (
              <tr><td colSpan={5} className="py-8 text-center text-gray-400">Cargando...</td></tr>
            ) : !data?.items.length ? (
              <tr><td colSpan={5} className="py-8 text-center text-gray-400">No hay usuarios</td></tr>
            ) : data.items.map((u) => (
              <tr key={u.id} className="hover:bg-gray-50">
                <td className="px-4 py-3 font-medium text-gray-900">{u.fullName}</td>
                <td className="px-4 py-3 text-gray-600">{u.email}</td>
                <td className="px-4 py-3">
                  <Badge variant={roleVariant[u.role] ?? 'secondary'}>
                    {roleLabel[u.role] ?? u.role}
                  </Badge>
                </td>
                <td className="px-4 py-3">
                  <span className={`inline-flex items-center gap-1.5 text-xs font-medium ${u.isActive ? 'text-green-700' : 'text-gray-400'}`}>
                    <span className={`h-1.5 w-1.5 rounded-full ${u.isActive ? 'bg-green-500' : 'bg-gray-300'}`} />
                    {u.isActive ? 'Activo' : 'Inactivo'}
                  </span>
                </td>
                <td className="px-4 py-3">
                  <button
                    onClick={() => toggleMutation.mutate({ id: u.id, activate: !u.isActive })}
                    disabled={toggleMutation.isPending}
                    className={`text-xs font-medium hover:underline ${u.isActive ? 'text-red-600' : 'text-green-600'}`}
                  >
                    {u.isActive ? 'Desactivar' : 'Activar'}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Modal crear usuario */}
      {showCreate && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="w-full max-w-lg rounded-xl bg-white p-6 shadow-xl">
            <h2 className="mb-5 text-lg font-semibold text-gray-900">Nuevo usuario</h2>
            <form onSubmit={form.handleSubmit((v) => createMutation.mutate(v))} className="space-y-4">

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="firstName">Nombre</Label>
                  <Input id="firstName" {...form.register('firstName')} />
                  {form.formState.errors.firstName && (
                    <p className="mt-1 text-xs text-red-500">{form.formState.errors.firstName.message}</p>
                  )}
                </div>
                <div>
                  <Label htmlFor="lastName">Apellido</Label>
                  <Input id="lastName" {...form.register('lastName')} />
                  {form.formState.errors.lastName && (
                    <p className="mt-1 text-xs text-red-500">{form.formState.errors.lastName.message}</p>
                  )}
                </div>
              </div>

              <div>
                <Label htmlFor="email">Correo electrónico</Label>
                <Input id="email" type="email" {...form.register('email')} />
                {form.formState.errors.email && (
                  <p className="mt-1 text-xs text-red-500">{form.formState.errors.email.message}</p>
                )}
              </div>

              <div>
                <Label htmlFor="password">Contraseña</Label>
                <Input id="password" type="password" {...form.register('password')} />
                {form.formState.errors.password && (
                  <p className="mt-1 text-xs text-red-500">{form.formState.errors.password.message}</p>
                )}
                <p className="mt-1 text-xs text-gray-400">Mínimo 8 caracteres, una mayúscula y un número</p>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="role">Rol</Label>
                  <select
                    id="role"
                    {...form.register('role')}
                    className="mt-1 h-9 w-full rounded-md border border-gray-300 bg-white px-3 text-sm"
                  >
                    <option value="Admin">Admin</option>
                    <option value="Supervisor">Supervisor</option>
                    <option value="Technician">Técnico</option>
                    <option value="Client">Cliente</option>
                  </select>
                </div>
                <div>
                  <Label htmlFor="phone">Teléfono (opcional)</Label>
                  <Input id="phone" {...form.register('phone')} placeholder="+57 300 000 0000" />
                </div>
              </div>

              {createMutation.isError && (
                <p className="text-sm text-red-500">Error al crear el usuario. Verifica los datos.</p>
              )}

              <div className="flex justify-end gap-3 pt-2">
                <Button type="button" variant="outline" onClick={() => { setShowCreate(false); form.reset() }}>
                  Cancelar
                </Button>
                <Button type="submit" disabled={createMutation.isPending}>
                  {createMutation.isPending ? 'Creando...' : 'Crear usuario'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}
