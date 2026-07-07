import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from '@tanstack/react-router'
import { api } from '@/lib/api'
import { Plus, ChevronRight, ClipboardList, Power } from 'lucide-react'

interface TemplateListItem {
  id: string
  name: string
  description: string | null
  inspectionTypeId: string | null
  inspectionTypeName: string | null
  isActive: boolean
  sectionCount: number
}

interface InspectionType {
  id: string
  name: string
}

export function ChecklistTemplatesPage() {
  const qc = useQueryClient()
  const navigate = useNavigate()
  const [showModal, setShowModal] = useState(false)
  const [form, setForm] = useState({ name: '', description: '', inspectionTypeId: '' })
  const [error, setError] = useState('')

  const { data: templates = [], isLoading } = useQuery({
    queryKey: ['checklist-templates'],
    queryFn: () => api.get<TemplateListItem[]>('/checklist-templates').then(r => r.data),
  })

  const { data: inspectionTypes = [] } = useQuery({
    queryKey: ['inspection-types'],
    queryFn: () => api.get<InspectionType[]>('/inspection-types').then(r => r.data),
  })

  const toggleMutation = useMutation({
    mutationFn: (id: string) => api.put(`/checklist-templates/${id}/toggle`),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['checklist-templates'] }),
  })

  const createMutation = useMutation({
    mutationFn: (data: { name: string; description?: string; inspectionTypeId?: string }) =>
      api.post('/checklist-templates', data),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['checklist-templates'] })
      setShowModal(false)
      setForm({ name: '', description: '', inspectionTypeId: '' })
      setError('')
    },
    onError: (err: any) => setError(err.response?.data?.message ?? 'Error al crear'),
  })

  function handleCreate() {
    if (!form.name.trim()) { setError('El nombre es requerido'); return }
    createMutation.mutate({
      name: form.name.trim(),
      description: form.description.trim() || undefined,
      inspectionTypeId: form.inspectionTypeId || undefined,
    })
  }

  return (
    <div className="p-8">
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Plantillas de checklist</h1>
          <p className="text-sm text-gray-500">Define los ítems de verificación por tipo de inspección</p>
        </div>
        <button
          onClick={() => setShowModal(true)}
          className="flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700"
        >
          <Plus className="h-4 w-4" /> Nueva plantilla
        </button>
      </div>

      {isLoading ? (
        <div className="text-center py-12 text-gray-400">Cargando...</div>
      ) : templates.length === 0 ? (
        <div className="flex flex-col items-center justify-center py-20 text-gray-400">
          <ClipboardList className="h-12 w-12 mb-3 opacity-40" />
          <p className="text-base font-medium">No hay plantillas creadas</p>
          <p className="text-sm">Crea una plantilla y agrégale secciones e ítems</p>
        </div>
      ) : (
        <div className="space-y-3">
          {templates.map(t => (
            <div
              key={t.id}
              className="flex items-center justify-between rounded-xl border border-gray-200 bg-white px-5 py-4 hover:border-blue-300 transition-colors"
            >
              <button
                className="flex items-center gap-4 flex-1 min-w-0 text-left"
                onClick={() => navigate({ to: `/checklist-templates/${t.id}` })}
              >
                <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-blue-50">
                  <ClipboardList className="h-5 w-5 text-blue-600" />
                </div>
                <div className="min-w-0">
                  <p className="font-semibold text-gray-900">{t.name}</p>
                  {t.description && <p className="text-sm text-gray-500 truncate">{t.description}</p>}
                  {t.inspectionTypeName && (
                    <p className="text-xs text-blue-600 mt-0.5">Vinculada a: {t.inspectionTypeName}</p>
                  )}
                </div>
              </button>
              <div className="flex items-center gap-3 shrink-0 ml-4">
                <span className="text-sm text-gray-500">{t.sectionCount} sección{t.sectionCount !== 1 ? 'es' : ''}</span>
                <span className={`text-xs font-medium px-2 py-0.5 rounded-full ${t.isActive ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-500'}`}>
                  {t.isActive ? 'Activa' : 'Inactiva'}
                </span>
                <button
                  onClick={() => toggleMutation.mutate(t.id)}
                  disabled={toggleMutation.isPending}
                  title={t.isActive ? 'Desactivar plantilla' : 'Activar plantilla'}
                  className={`flex items-center gap-1 rounded-lg border px-2.5 py-1 text-xs font-medium disabled:opacity-50 transition-colors ${
                    t.isActive
                      ? 'border-red-200 bg-red-50 text-red-600 hover:bg-red-100'
                      : 'border-green-200 bg-green-50 text-green-700 hover:bg-green-100'
                  }`}
                >
                  <Power className="h-3.5 w-3.5" />
                  {t.isActive ? 'Desactivar' : 'Activar'}
                </button>
                <ChevronRight className="h-4 w-4 text-gray-400" />
              </div>
            </div>
          ))}
        </div>
      )}

      {showModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="w-full max-w-md rounded-2xl bg-white p-6 shadow-xl">
            <h2 className="text-lg font-bold text-gray-900 mb-4">Nueva plantilla de checklist</h2>
            {error && <p className="mb-3 rounded-lg bg-red-50 px-3 py-2 text-sm text-red-600">{error}</p>}

            <div className="space-y-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Nombre *</label>
                <input
                  className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
                  value={form.name}
                  onChange={e => setForm(f => ({ ...f, name: e.target.value }))}
                  placeholder="Ej: Inspección residencial básica"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Descripción</label>
                <textarea
                  className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none resize-none"
                  rows={2}
                  value={form.description}
                  onChange={e => setForm(f => ({ ...f, description: e.target.value }))}
                  placeholder="Descripción opcional"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Tipo de inspección (opcional)</label>
                <select
                  className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
                  value={form.inspectionTypeId}
                  onChange={e => setForm(f => ({ ...f, inspectionTypeId: e.target.value }))}
                >
                  <option value="">— Sin vincular —</option>
                  {inspectionTypes.map(t => (
                    <option key={t.id} value={t.id}>{t.name}</option>
                  ))}
                </select>
              </div>
            </div>

            <div className="mt-5 flex justify-end gap-3">
              <button onClick={() => { setShowModal(false); setError('') }} className="rounded-lg px-4 py-2 text-sm font-medium text-gray-600 hover:bg-gray-100">
                Cancelar
              </button>
              <button
                onClick={handleCreate}
                disabled={createMutation.isPending}
                className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700 disabled:opacity-60"
              >
                {createMutation.isPending ? 'Creando...' : 'Crear plantilla'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
