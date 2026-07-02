import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { api } from '@/lib/api'
import type { PagedResult, ClientDto } from '@/types/api'

interface ContractDto {
  id: string
  contractNumber: string
  clientId: string
  clientName: string
  startDate: string
  endDate: string
  status: string
  notes?: string
}

const statusLabel: Record<string, string> = {
  Active: 'Activo', Suspended: 'Suspendido', Cancelled: 'Cancelado',
}
const statusVariant: Record<string, 'success' | 'warning' | 'destructive'> = {
  Active: 'success', Suspended: 'warning', Cancelled: 'destructive',
}

const schema = z.object({
  clientId: z.string().min(1, 'Selecciona un cliente'),
  contractNumber: z.string().min(1, 'Requerido'),
  startDate: z.string().min(1, 'Requerido'),
  endDate: z.string().min(1, 'Requerido'),
  notes: z.string().optional(),
}).refine((d) => new Date(d.endDate) > new Date(d.startDate), {
  message: 'La fecha de fin debe ser posterior a la de inicio',
  path: ['endDate'],
})

type FormData = z.infer<typeof schema>

export function ContractsPage() {
  const qc = useQueryClient()
  const [showCreate, setShowCreate] = useState(false)
  const [clientFilter, setClientFilter] = useState('')

  const { data: contracts, isLoading } = useQuery({
    queryKey: ['contracts', clientFilter],
    queryFn: () =>
      api.get<ContractDto[]>(`/contracts${clientFilter ? `?clientId=${clientFilter}` : ''}`).then((r) => r.data),
  })

  const { data: clients } = useQuery({
    queryKey: ['clients-list'],
    queryFn: () =>
      api.get<PagedResult<ClientDto>>('/clients?pageSize=100&isActive=true').then((r) => r.data.items),
  })

  const form = useForm<FormData>({ resolver: zodResolver(schema) })

  const createMutation = useMutation({
    mutationFn: (data: FormData) =>
      api.post('/contracts', {
        clientId: data.clientId,
        contractNumber: data.contractNumber,
        startDate: new Date(data.startDate).toISOString(),
        endDate: new Date(data.endDate).toISOString(),
        notes: data.notes || null,
      }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['contracts'] })
      setShowCreate(false)
      form.reset()
    },
  })

  return (
    <div className="p-8">
      <div className="mb-6 flex items-start justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Contratos</h1>
          <p className="text-sm text-gray-500">{contracts?.length ?? 0} contratos registrados</p>
        </div>
        <Button onClick={() => setShowCreate(true)}>+ Nuevo contrato</Button>
      </div>

      <div className="mb-4">
        <select
          value={clientFilter}
          onChange={(e) => setClientFilter(e.target.value)}
          className="h-9 rounded-md border border-gray-300 bg-white px-3 text-sm"
        >
          <option value="">Todos los clientes</option>
          {clients?.map((c) => (
            <option key={c.id} value={c.id}>{c.businessName}</option>
          ))}
        </select>
      </div>

      <div className="rounded-lg border border-gray-200 bg-white">
        <table className="w-full text-sm">
          <thead className="border-b border-gray-200 bg-gray-50">
            <tr>
              <th className="px-4 py-3 text-left font-medium text-gray-600">N° Contrato</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Cliente</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Inicio</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Fin</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Estado</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {isLoading ? (
              <tr><td colSpan={5} className="py-8 text-center text-gray-400">Cargando...</td></tr>
            ) : !contracts?.length ? (
              <tr><td colSpan={5} className="py-8 text-center text-gray-400">No hay contratos registrados</td></tr>
            ) : contracts.map((c) => (
              <tr key={c.id} className="hover:bg-gray-50">
                <td className="px-4 py-3 font-medium text-gray-900">{c.contractNumber}</td>
                <td className="px-4 py-3 text-gray-600">{c.clientName}</td>
                <td className="px-4 py-3 text-gray-600">{new Date(c.startDate).toLocaleDateString('es-CO')}</td>
                <td className="px-4 py-3 text-gray-600">{new Date(c.endDate).toLocaleDateString('es-CO')}</td>
                <td className="px-4 py-3">
                  <Badge variant={statusVariant[c.status] ?? 'secondary'}>
                    {statusLabel[c.status] ?? c.status}
                  </Badge>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {showCreate && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="w-full max-w-lg rounded-xl bg-white p-6 shadow-xl">
            <h2 className="mb-5 text-lg font-semibold text-gray-900">Nuevo contrato</h2>
            <form onSubmit={form.handleSubmit((v) => createMutation.mutate(v))} className="space-y-4">

              <div>
                <Label htmlFor="clientId">Cliente</Label>
                <select
                  id="clientId"
                  {...form.register('clientId')}
                  className="mt-1 h-9 w-full rounded-md border border-gray-300 bg-white px-3 text-sm"
                >
                  <option value="">Seleccionar cliente...</option>
                  {clients?.map((c) => (
                    <option key={c.id} value={c.id}>{c.businessName}</option>
                  ))}
                </select>
                {form.formState.errors.clientId && (
                  <p className="mt-1 text-xs text-red-500">{form.formState.errors.clientId.message}</p>
                )}
              </div>

              <div>
                <Label htmlFor="contractNumber">N° de contrato</Label>
                <Input id="contractNumber" placeholder="CTR-2026-001" {...form.register('contractNumber')} />
                {form.formState.errors.contractNumber && (
                  <p className="mt-1 text-xs text-red-500">{form.formState.errors.contractNumber.message}</p>
                )}
              </div>

              <div className="grid grid-cols-2 gap-3">
                <div>
                  <Label htmlFor="startDate">Fecha de inicio</Label>
                  <Input id="startDate" type="date" {...form.register('startDate')} />
                  {form.formState.errors.startDate && (
                    <p className="mt-1 text-xs text-red-500">{form.formState.errors.startDate.message}</p>
                  )}
                </div>
                <div>
                  <Label htmlFor="endDate">Fecha de fin</Label>
                  <Input id="endDate" type="date" {...form.register('endDate')} />
                  {form.formState.errors.endDate && (
                    <p className="mt-1 text-xs text-red-500">{form.formState.errors.endDate.message}</p>
                  )}
                </div>
              </div>

              <div>
                <Label htmlFor="notes">Notas (opcional)</Label>
                <Input id="notes" placeholder="Observaciones del contrato..." {...form.register('notes')} />
              </div>

              {createMutation.isError && (
                <p className="text-sm text-red-500">Error al crear el contrato. Verifica los datos.</p>
              )}

              <div className="flex justify-end gap-3 pt-2">
                <Button type="button" variant="outline" onClick={() => { setShowCreate(false); form.reset() }}>
                  Cancelar
                </Button>
                <Button type="submit" disabled={createMutation.isPending}>
                  {createMutation.isPending ? 'Creando...' : 'Crear contrato'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}
