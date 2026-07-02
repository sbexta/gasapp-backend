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
import type { PagedResult, WorkOrderDto, LocationDto, InspectionTypeDto, UserDto } from '@/types/api'

const statusLabel: Record<string, string> = {
  Draft: 'Borrador', Scheduled: 'Programada', Assigned: 'Asignada',
  InProgress: 'En progreso', Completed: 'Completada', Cancelled: 'Cancelada',
}
const statusVariant: Record<string, 'default' | 'success' | 'warning' | 'destructive' | 'secondary'> = {
  Draft: 'secondary', Scheduled: 'default', Assigned: 'warning',
  InProgress: 'default', Completed: 'success', Cancelled: 'destructive',
}

const createSchema = z.object({
  orderNumber: z.string().min(1, 'Requerido'),
  locationId: z.string().min(1, 'Selecciona una sede'),
  inspectionTypeId: z.string().min(1, 'Selecciona un tipo'),
  scheduledDate: z.string().min(1, 'Requerido'),
  notes: z.string().optional(),
})
type CreateForm = z.infer<typeof createSchema>

const assignSchema = z.object({
  technicianId: z.string().min(1, 'Selecciona un técnico'),
})
type AssignForm = z.infer<typeof assignSchema>

export function WorkOrdersPage() {
  const qc = useQueryClient()
  const [statusFilter, setStatusFilter] = useState('')
  const [showCreate, setShowCreate] = useState(false)
  const [assigningId, setAssigningId] = useState<string | null>(null)

  const { data, isLoading } = useQuery({
    queryKey: ['work-orders', statusFilter],
    queryFn: () =>
      api.get<PagedResult<WorkOrderDto>>(
        `/work-orders?pageSize=50${statusFilter ? `&status=${statusFilter}` : ''}`
      ).then((r) => r.data),
  })

  const { data: locations } = useQuery({
    queryKey: ['locations'],
    queryFn: () => api.get<LocationDto[]>('/locations').then((r) => r.data),
    enabled: showCreate,
  })

  const { data: inspectionTypes } = useQuery({
    queryKey: ['inspection-types'],
    queryFn: () => api.get<InspectionTypeDto[]>('/inspection-types').then((r) => r.data),
    enabled: showCreate,
  })

  const { data: technicians } = useQuery({
    queryKey: ['technicians'],
    queryFn: () =>
      api.get<PagedResult<UserDto>>('/users?role=Technician&pageSize=100&isActive=true').then((r) => r.data),
    enabled: !!assigningId,
  })

  const createForm = useForm<CreateForm>({ resolver: zodResolver(createSchema) })
  const assignForm = useForm<AssignForm>({ resolver: zodResolver(assignSchema) })

  const createMutation = useMutation({
    mutationFn: (data: CreateForm) =>
      api.post('/work-orders', {
        orderNumber: data.orderNumber,
        locationId: data.locationId,
        inspectionTypeId: data.inspectionTypeId,
        scheduledDate: new Date(data.scheduledDate).toISOString(),
        notes: data.notes || null,
      }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['work-orders'] })
      setShowCreate(false)
      createForm.reset()
    },
  })

  const assignMutation = useMutation({
    mutationFn: ({ id, technicianId }: { id: string; technicianId: string }) =>
      api.post(`/work-orders/${id}/assign`, { technicianId }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['work-orders'] })
      setAssigningId(null)
      assignForm.reset()
    },
  })

  const startMutation = useMutation({
    mutationFn: (id: string) => api.post(`/work-orders/${id}/start`),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['work-orders'] }),
  })

  function onCreateSubmit(values: CreateForm) {
    createMutation.mutate(values)
  }

  function onAssignSubmit(values: AssignForm) {
    if (!assigningId) return
    assignMutation.mutate({ id: assigningId, technicianId: values.technicianId })
  }

  return (
    <div className="p-8">
      <div className="mb-6 flex items-start justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Órdenes de trabajo</h1>
          <p className="text-sm text-gray-500">{data?.totalCount ?? 0} órdenes registradas</p>
        </div>
        <Button onClick={() => setShowCreate(true)}>+ Nueva orden</Button>
      </div>

      <div className="mb-4 flex items-center gap-3">
        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
          className="h-9 rounded-md border border-gray-300 bg-white px-3 text-sm"
        >
          <option value="">Todos los estados</option>
          {Object.entries(statusLabel).map(([val, label]) => (
            <option key={val} value={val}>{label}</option>
          ))}
        </select>
      </div>

      <div className="rounded-lg border border-gray-200 bg-white">
        <table className="w-full text-sm">
          <thead className="border-b border-gray-200 bg-gray-50">
            <tr>
              <th className="px-4 py-3 text-left font-medium text-gray-600">N° Orden</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Fecha programada</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Estado</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Acciones</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {isLoading ? (
              <tr><td colSpan={4} className="py-8 text-center text-gray-400">Cargando...</td></tr>
            ) : !data?.items.length ? (
              <tr><td colSpan={4} className="py-8 text-center text-gray-400">No hay órdenes registradas</td></tr>
            ) : data.items.map((o) => (
              <tr key={o.id} className="hover:bg-gray-50">
                <td className="px-4 py-3 font-medium text-gray-900">{o.orderNumber}</td>
                <td className="px-4 py-3 text-gray-600">
                  {new Date(o.scheduledDate).toLocaleDateString('es-CO')}
                </td>
                <td className="px-4 py-3">
                  <Badge variant={statusVariant[o.status] ?? 'secondary'}>
                    {statusLabel[o.status] ?? o.status}
                  </Badge>
                </td>
                <td className="px-4 py-3 flex items-center gap-3">
                  {(o.status === 'Draft' || o.status === 'Scheduled') && (
                    <button
                      onClick={() => { setAssigningId(o.id); assignForm.reset() }}
                      className="text-xs font-medium text-blue-600 hover:underline"
                    >
                      Asignar técnico
                    </button>
                  )}
                  {o.status === 'Assigned' && (
                    <button
                      onClick={() => startMutation.mutate(o.id)}
                      disabled={startMutation.isPending}
                      className="text-xs font-medium text-purple-600 hover:underline disabled:opacity-50"
                    >
                      Iniciar inspección
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Modal crear orden */}
      {showCreate && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="w-full max-w-lg rounded-xl bg-white p-6 shadow-xl">
            <h2 className="mb-5 text-lg font-semibold text-gray-900">Nueva orden de trabajo</h2>
            <form onSubmit={createForm.handleSubmit(onCreateSubmit)} className="space-y-4">
              <div>
                <Label htmlFor="orderNumber">N° de orden</Label>
                <Input id="orderNumber" placeholder="OT-2026-001" {...createForm.register('orderNumber')} />
                {createForm.formState.errors.orderNumber && (
                  <p className="mt-1 text-xs text-red-500">{createForm.formState.errors.orderNumber.message}</p>
                )}
              </div>

              <div>
                <Label htmlFor="locationId">Sede</Label>
                <select
                  id="locationId"
                  {...createForm.register('locationId')}
                  className="mt-1 h-9 w-full rounded-md border border-gray-300 bg-white px-3 text-sm"
                >
                  <option value="">Seleccionar sede...</option>
                  {locations?.map((l) => (
                    <option key={l.id} value={l.id}>
                      {l.name} — {l.clientName} ({l.municipality})
                    </option>
                  ))}
                </select>
                {createForm.formState.errors.locationId && (
                  <p className="mt-1 text-xs text-red-500">{createForm.formState.errors.locationId.message}</p>
                )}
              </div>

              <div>
                <Label htmlFor="inspectionTypeId">Tipo de inspección</Label>
                <select
                  id="inspectionTypeId"
                  {...createForm.register('inspectionTypeId')}
                  className="mt-1 h-9 w-full rounded-md border border-gray-300 bg-white px-3 text-sm"
                >
                  <option value="">Seleccionar tipo...</option>
                  {inspectionTypes?.map((t) => (
                    <option key={t.id} value={t.id}>{t.name}</option>
                  ))}
                </select>
                {createForm.formState.errors.inspectionTypeId && (
                  <p className="mt-1 text-xs text-red-500">{createForm.formState.errors.inspectionTypeId.message}</p>
                )}
              </div>

              <div>
                <Label htmlFor="scheduledDate">Fecha programada</Label>
                <Input id="scheduledDate" type="date" {...createForm.register('scheduledDate')} />
                {createForm.formState.errors.scheduledDate && (
                  <p className="mt-1 text-xs text-red-500">{createForm.formState.errors.scheduledDate.message}</p>
                )}
              </div>

              <div>
                <Label htmlFor="notes">Notas (opcional)</Label>
                <Input id="notes" placeholder="Observaciones adicionales..." {...createForm.register('notes')} />
              </div>

              {createMutation.isError && (
                <p className="text-sm text-red-500">Error al crear la orden. Verifica los datos.</p>
              )}

              <div className="flex justify-end gap-3 pt-2">
                <Button type="button" variant="outline" onClick={() => { setShowCreate(false); createForm.reset() }}>
                  Cancelar
                </Button>
                <Button type="submit" disabled={createMutation.isPending}>
                  {createMutation.isPending ? 'Creando...' : 'Crear orden'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Modal asignar técnico */}
      {assigningId && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="w-full max-w-md rounded-xl bg-white p-6 shadow-xl">
            <h2 className="mb-5 text-lg font-semibold text-gray-900">Asignar técnico</h2>
            <form onSubmit={assignForm.handleSubmit(onAssignSubmit)} className="space-y-4">
              <div>
                <Label htmlFor="technicianId">Técnico</Label>
                <select
                  id="technicianId"
                  {...assignForm.register('technicianId')}
                  className="mt-1 h-9 w-full rounded-md border border-gray-300 bg-white px-3 text-sm"
                >
                  <option value="">Seleccionar técnico...</option>
                  {technicians?.items.map((u) => (
                    <option key={u.id} value={u.id}>{u.fullName} — {u.email}</option>
                  ))}
                </select>
                {assignForm.formState.errors.technicianId && (
                  <p className="mt-1 text-xs text-red-500">{assignForm.formState.errors.technicianId.message}</p>
                )}
              </div>

              {assignMutation.isError && (
                <p className="text-sm text-red-500">Error al asignar. Intenta de nuevo.</p>
              )}

              <div className="flex justify-end gap-3 pt-2">
                <Button type="button" variant="outline" onClick={() => { setAssigningId(null); assignForm.reset() }}>
                  Cancelar
                </Button>
                <Button type="submit" disabled={assignMutation.isPending}>
                  {assignMutation.isPending ? 'Asignando...' : 'Asignar'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}
