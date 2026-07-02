import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { api } from '@/lib/api'
import type { LocationDto, PagedResult, ClientDto, ClientDetailDto } from '@/types/api'

const schema = z.object({
  clientId: z.string().min(1, 'Selecciona un cliente'),
  contractId: z.string().min(1, 'Selecciona un contrato'),
  name: z.string().min(1, 'Requerido'),
  address: z.string().min(1, 'Requerido'),
  municipality: z.string().min(1, 'Requerido'),
  department: z.string().min(1, 'Requerido'),
})
type FormData = z.infer<typeof schema>

export function LocationsPage() {
  const qc = useQueryClient()
  const [showCreate, setShowCreate] = useState(false)

  const { data: locations, isLoading } = useQuery({
    queryKey: ['locations'],
    queryFn: () => api.get<LocationDto[]>('/locations').then((r) => r.data),
  })

  const { data: clients } = useQuery({
    queryKey: ['clients-list'],
    queryFn: () =>
      api.get<PagedResult<ClientDto>>('/clients?pageSize=100&isActive=true').then((r) => r.data.items),
    enabled: showCreate,
  })

  const form = useForm<FormData>({ resolver: zodResolver(schema) })
  const selectedClientId = form.watch('clientId')

  const { data: clientDetail } = useQuery({
    queryKey: ['client-detail', selectedClientId],
    queryFn: () =>
      api.get<ClientDetailDto>(`/clients/${selectedClientId}`).then((r) => r.data),
    enabled: !!selectedClientId,
  })

  const createMutation = useMutation({
    mutationFn: (data: FormData) =>
      api.post('/locations', {
        contractId: data.contractId,
        name: data.name,
        address: data.address,
        municipality: data.municipality,
        department: data.department,
        latitude: null,
        longitude: null,
      }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['locations'] })
      setShowCreate(false)
      form.reset()
    },
  })

  function onSubmit(values: FormData) {
    createMutation.mutate(values)
  }

  return (
    <div className="p-8">
      <div className="mb-6 flex items-start justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Sedes</h1>
          <p className="text-sm text-gray-500">{locations?.length ?? 0} sedes registradas</p>
        </div>
        <Button onClick={() => setShowCreate(true)}>+ Nueva sede</Button>
      </div>

      <div className="rounded-lg border border-gray-200 bg-white">
        <table className="w-full text-sm">
          <thead className="border-b border-gray-200 bg-gray-50">
            <tr>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Nombre</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Cliente</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Dirección</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Municipio</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Departamento</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {isLoading ? (
              <tr><td colSpan={5} className="py-8 text-center text-gray-400">Cargando...</td></tr>
            ) : !locations?.length ? (
              <tr><td colSpan={5} className="py-8 text-center text-gray-400">No hay sedes registradas</td></tr>
            ) : locations.map((l) => (
              <tr key={l.id} className="hover:bg-gray-50">
                <td className="px-4 py-3 font-medium text-gray-900">{l.name}</td>
                <td className="px-4 py-3 text-gray-600">{l.clientName}</td>
                <td className="px-4 py-3 text-gray-600">{l.address}</td>
                <td className="px-4 py-3 text-gray-600">{l.municipality}</td>
                <td className="px-4 py-3 text-gray-600">{l.department}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {showCreate && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="w-full max-w-lg rounded-xl bg-white p-6 shadow-xl">
            <h2 className="mb-5 text-lg font-semibold text-gray-900">Nueva sede</h2>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">

              <div>
                <Label htmlFor="clientId">Cliente</Label>
                <select
                  id="clientId"
                  {...form.register('clientId')}
                  onChange={(e) => {
                    form.setValue('clientId', e.target.value)
                    form.setValue('contractId', '')
                  }}
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
                <Label htmlFor="contractId">Contrato</Label>
                <select
                  id="contractId"
                  {...form.register('contractId')}
                  disabled={!selectedClientId}
                  className="mt-1 h-9 w-full rounded-md border border-gray-300 bg-white px-3 text-sm disabled:opacity-50"
                >
                  <option value="">Seleccionar contrato...</option>
                  {clientDetail?.contracts.map((c) => (
                    <option key={c.id} value={c.id}>
                      {c.contractNumber} — {new Date(c.startDate).toLocaleDateString('es-CO')} / {new Date(c.endDate).toLocaleDateString('es-CO')}
                    </option>
                  ))}
                </select>
                {form.formState.errors.contractId && (
                  <p className="mt-1 text-xs text-red-500">{form.formState.errors.contractId.message}</p>
                )}
              </div>

              <div className="grid grid-cols-2 gap-3">
                <div>
                  <Label htmlFor="name">Nombre de la sede</Label>
                  <Input id="name" placeholder="Sede Principal" {...form.register('name')} />
                  {form.formState.errors.name && (
                    <p className="mt-1 text-xs text-red-500">{form.formState.errors.name.message}</p>
                  )}
                </div>
                <div>
                  <Label htmlFor="address">Dirección</Label>
                  <Input id="address" placeholder="Calle 10 # 5-20" {...form.register('address')} />
                  {form.formState.errors.address && (
                    <p className="mt-1 text-xs text-red-500">{form.formState.errors.address.message}</p>
                  )}
                </div>
              </div>

              <div className="grid grid-cols-2 gap-3">
                <div>
                  <Label htmlFor="municipality">Municipio</Label>
                  <Input id="municipality" placeholder="Bogotá" {...form.register('municipality')} />
                  {form.formState.errors.municipality && (
                    <p className="mt-1 text-xs text-red-500">{form.formState.errors.municipality.message}</p>
                  )}
                </div>
                <div>
                  <Label htmlFor="department">Departamento</Label>
                  <Input id="department" placeholder="Cundinamarca" {...form.register('department')} />
                  {form.formState.errors.department && (
                    <p className="mt-1 text-xs text-red-500">{form.formState.errors.department.message}</p>
                  )}
                </div>
              </div>

              {createMutation.isError && (
                <p className="text-sm text-red-500">Error al crear la sede. Verifica los datos.</p>
              )}

              <div className="flex justify-end gap-3 pt-2">
                <Button type="button" variant="outline" onClick={() => { setShowCreate(false); form.reset() }}>
                  Cancelar
                </Button>
                <Button type="submit" disabled={createMutation.isPending}>
                  {createMutation.isPending ? 'Creando...' : 'Crear sede'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}
