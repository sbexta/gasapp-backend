import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Plus, Search, Loader2 } from 'lucide-react'
import * as Dialog from '@radix-ui/react-dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { api } from '@/lib/api'
import type { PagedResult, ClientDto } from '@/types/api'

const schema = z.object({
  businessName: z.string().min(1, 'Requerido'),
  nit: z.string().regex(/^\d{9}-\d$/, 'Formato: 123456789-0'),
  type: z.enum(['0', '1', '2']),
  contactName: z.string().optional(),
  contactPhone: z.string().optional(),
  contactEmail: z.string().email('Email inválido').optional().or(z.literal('')),
})
type FormData = z.infer<typeof schema>

const clientTypeLabel = ['Residencial', 'Comercial', 'Industrial']

export function ClientsPage() {
  const [search, setSearch] = useState('')
  const [open, setOpen] = useState(false)
  const queryClient = useQueryClient()

  const { data, isLoading } = useQuery({
    queryKey: ['clients', search],
    queryFn: () =>
      api.get<PagedResult<ClientDto>>(`/clients?search=${search}&pageSize=50`).then((r) => r.data),
  })

  const { register, handleSubmit, formState: { errors, isSubmitting }, reset, setError } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: { type: '0' },
  })

  const createMutation = useMutation({
    mutationFn: (body: FormData) =>
      api.post('/clients', { ...body, type: Number(body.type) }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clients'] })
      setOpen(false)
      reset()
    },
    onError: (err: any) => {
      const msg = err.response?.data?.message ?? 'Error al crear cliente'
      setError('root', { message: msg })
    },
  })

  return (
    <div className="p-8">
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Clientes</h1>
          <p className="text-sm text-gray-500">{data?.total ?? 0} clientes registrados</p>
        </div>
        <Dialog.Root open={open} onOpenChange={setOpen}>
          <Dialog.Trigger asChild>
            <Button><Plus className="h-4 w-4" /> Nuevo cliente</Button>
          </Dialog.Trigger>
          <Dialog.Portal>
            <Dialog.Overlay className="fixed inset-0 bg-black/40" />
            <Dialog.Content className="fixed left-1/2 top-1/2 w-full max-w-md -translate-x-1/2 -translate-y-1/2 rounded-lg bg-white p-6 shadow-xl">
              <Dialog.Title className="mb-4 text-lg font-semibold">Nuevo cliente</Dialog.Title>
              <form onSubmit={handleSubmit((d) => createMutation.mutate(d))} className="space-y-4">
                <div className="space-y-1.5">
                  <Label>Razón social</Label>
                  <Input placeholder="ACME S.A.S" {...register('businessName')} />
                  {errors.businessName && <p className="text-xs text-red-500">{errors.businessName.message}</p>}
                </div>
                <div className="space-y-1.5">
                  <Label>NIT</Label>
                  <Input placeholder="900123456-1" {...register('nit')} />
                  {errors.nit && <p className="text-xs text-red-500">{errors.nit.message}</p>}
                </div>
                <div className="space-y-1.5">
                  <Label>Tipo</Label>
                  <select {...register('type')} className="flex h-9 w-full rounded-md border border-gray-300 bg-white px-3 py-1 text-sm">
                    <option value="0">Residencial</option>
                    <option value="1">Comercial</option>
                    <option value="2">Industrial</option>
                  </select>
                </div>
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-1.5">
                    <Label>Nombre contacto</Label>
                    <Input {...register('contactName')} />
                  </div>
                  <div className="space-y-1.5">
                    <Label>Teléfono</Label>
                    <Input {...register('contactPhone')} />
                  </div>
                </div>
                <div className="space-y-1.5">
                  <Label>Email contacto</Label>
                  <Input type="email" {...register('contactEmail')} />
                  {errors.contactEmail && <p className="text-xs text-red-500">{errors.contactEmail.message}</p>}
                </div>
                {errors.root && (
                  <p className="rounded-md bg-red-50 px-3 py-2 text-xs text-red-600">{errors.root.message}</p>
                )}
                <div className="flex justify-end gap-2">
                  <Dialog.Close asChild>
                    <Button type="button" variant="outline">Cancelar</Button>
                  </Dialog.Close>
                  <Button type="submit" disabled={isSubmitting}>
                    {isSubmitting && <Loader2 className="h-4 w-4 animate-spin" />}
                    Guardar
                  </Button>
                </div>
              </form>
            </Dialog.Content>
          </Dialog.Portal>
        </Dialog.Root>
      </div>

      <div className="mb-4 flex items-center gap-2">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
          <Input
            placeholder="Buscar por nombre o NIT..."
            className="pl-9"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
      </div>

      <div className="rounded-lg border border-gray-200 bg-white">
        <table className="w-full text-sm">
          <thead className="border-b border-gray-200 bg-gray-50">
            <tr>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Razón social</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">NIT</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Tipo</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Contacto</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Estado</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {isLoading ? (
              <tr><td colSpan={5} className="py-8 text-center text-gray-400">Cargando...</td></tr>
            ) : !data?.items.length ? (
              <tr><td colSpan={5} className="py-8 text-center text-gray-400">No hay clientes registrados</td></tr>
            ) : data.items.map((c) => (
              <tr key={c.id} className="hover:bg-gray-50">
                <td className="px-4 py-3 font-medium text-gray-900">{c.businessName}</td>
                <td className="px-4 py-3 text-gray-600">{c.nit}</td>
                <td className="px-4 py-3 text-gray-600">{clientTypeLabel[['Residential','Commercial','Industrial'].indexOf(c.type)] ?? c.type}</td>
                <td className="px-4 py-3 text-gray-600">{c.contactName ?? '—'}</td>
                <td className="px-4 py-3">
                  <Badge variant={c.isActive ? 'success' : 'secondary'}>
                    {c.isActive ? 'Activo' : 'Inactivo'}
                  </Badge>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
