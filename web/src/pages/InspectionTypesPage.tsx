import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { api } from '@/lib/api'
import type { InspectionTypeDto } from '@/types/api'

const schema = z.object({
  name: z.string().min(1, 'Requerido'),
  description: z.string().optional(),
  requiresCertificate: z.boolean(),
})
type FormData = z.infer<typeof schema>

export function InspectionTypesPage() {
  const qc = useQueryClient()
  const [showCreate, setShowCreate] = useState(false)

  const { data: types, isLoading } = useQuery({
    queryKey: ['inspection-types'],
    queryFn: () => api.get<InspectionTypeDto[]>('/inspection-types').then((r) => r.data),
  })

  const form = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: { requiresCertificate: true },
  })

  const createMutation = useMutation({
    mutationFn: (data: FormData) =>
      api.post('/inspection-types', {
        name: data.name,
        description: data.description || null,
        requiresCertificate: data.requiresCertificate,
      }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['inspection-types'] })
      setShowCreate(false)
      form.reset({ requiresCertificate: true })
    },
  })

  function onSubmit(values: FormData) {
    createMutation.mutate(values)
  }

  return (
    <div className="p-8">
      <div className="mb-6 flex items-start justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Tipos de inspección</h1>
          <p className="text-sm text-gray-500">{types?.length ?? 0} tipos registrados</p>
        </div>
        <Button onClick={() => setShowCreate(true)}>+ Nuevo tipo</Button>
      </div>

      <div className="rounded-lg border border-gray-200 bg-white">
        <table className="w-full text-sm">
          <thead className="border-b border-gray-200 bg-gray-50">
            <tr>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Nombre</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Descripción</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Requiere certificado</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {isLoading ? (
              <tr><td colSpan={3} className="py-8 text-center text-gray-400">Cargando...</td></tr>
            ) : !types?.length ? (
              <tr><td colSpan={3} className="py-8 text-center text-gray-400">No hay tipos registrados</td></tr>
            ) : types.map((t) => (
              <tr key={t.id} className="hover:bg-gray-50">
                <td className="px-4 py-3 font-medium text-gray-900">{t.name}</td>
                <td className="px-4 py-3 text-gray-600">{t.description ?? '—'}</td>
                <td className="px-4 py-3">
                  <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${
                    t.requiresCertificate ? 'bg-green-50 text-green-700' : 'bg-gray-100 text-gray-600'
                  }`}>
                    {t.requiresCertificate ? 'Sí' : 'No'}
                  </span>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {showCreate && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="w-full max-w-md rounded-xl bg-white p-6 shadow-xl">
            <h2 className="mb-5 text-lg font-semibold text-gray-900">Nuevo tipo de inspección</h2>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">

              <div>
                <Label htmlFor="name">Nombre</Label>
                <Input id="name" placeholder="Inspección residencial de gas" {...form.register('name')} />
                {form.formState.errors.name && (
                  <p className="mt-1 text-xs text-red-500">{form.formState.errors.name.message}</p>
                )}
              </div>

              <div>
                <Label htmlFor="description">Descripción (opcional)</Label>
                <Input id="description" placeholder="Descripción del tipo de inspección..." {...form.register('description')} />
              </div>

              <div className="flex items-center gap-3">
                <input
                  id="requiresCertificate"
                  type="checkbox"
                  {...form.register('requiresCertificate')}
                  className="h-4 w-4 rounded border-gray-300 text-blue-600"
                />
                <Label htmlFor="requiresCertificate" className="cursor-pointer">
                  Requiere emisión de certificado
                </Label>
              </div>

              {createMutation.isError && (
                <p className="text-sm text-red-500">Error al crear el tipo. Verifica los datos.</p>
              )}

              <div className="flex justify-end gap-3 pt-2">
                <Button type="button" variant="outline" onClick={() => { setShowCreate(false); form.reset({ requiresCertificate: true }) }}>
                  Cancelar
                </Button>
                <Button type="submit" disabled={createMutation.isPending}>
                  {createMutation.isPending ? 'Creando...' : 'Crear tipo'}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}
