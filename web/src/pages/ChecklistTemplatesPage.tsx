import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from '@tanstack/react-router'
import { api } from '@/lib/api'
import { Plus, ChevronRight, ClipboardList } from 'lucide-react'

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
            <button
              key={t.id}
              onClick={() => navigate({ to: `/checklist-templates/${t.id}` })}
              className="w-full flex items-center justify-between rounded-xl border border-gray-200 bg-white px-5 py-4 hover:border-blue-300 hover:bg-blue-50 transition-colors text-left"
            >
              <div className="flex items-center gap-4">
                <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-blue-50">
                  <ClipboardList className="h-5 w-5 text-blue-600" />
                </div>
                <div>
                  <p className="font-semibold text-gray-900">{t.name}</p>
                  {t.description && <p className="text-sm text-gray-500">{t.description}</p>}
                  {t.inspectionTypeName && (
                    <p className="text-xs text-blue-600 mt-0.5">Vinculada a: {t.inspectionTypeName}</p>
                  )}
                </div>
              </div>
              <div className="flex items-center gap-4">
                <span className="text-sm text-gray-500">{t.sectionCount} sección{t.sectionCount !== 1 ? 'es' : ''}</span>
                <span className={`text-xs font-medium px-2 py-0.5 rounded-full ${t.isActive ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-500'}`}>
                  {t.isActive ? 'Activa' : 'Inactiva'}
                </span>
                <ChevronRight className="h-4 w-4 text-gray-400" />
              </div>
            </button>
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
