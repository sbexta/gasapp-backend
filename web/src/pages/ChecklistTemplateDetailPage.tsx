import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useParams, useNavigate } from '@tanstack/react-router'
import { api } from '@/lib/api'
import { Plus, ChevronLeft, Trash2 } from 'lucide-react'

interface TemplateDetail {
  id: string
  name: string
  description: string | null
  inspectionTypeId: string | null
  isActive: boolean
  version: number
  sections: SectionDetail[]
}

interface SectionDetail {
  id: string
  name: string
  order: number
  items: ItemDetail[]
}

interface ItemDetail {
  id: string
  question: string
  itemType: string
  isRequired: boolean
  order: number
  helpText: string | null
}

const itemTypeLabel: Record<string, string> = {
  YesNo: 'Sí/No',
  Text: 'Texto',
  Numeric: 'Numérico',
  Photo: 'Foto',
  Signature: 'Firma',
}

const itemTypeColor: Record<string, string> = {
  YesNo: 'bg-green-100 text-green-700',
  Text: 'bg-blue-100 text-blue-700',
  Numeric: 'bg-purple-100 text-purple-700',
  Photo: 'bg-orange-100 text-orange-700',
  Signature: 'bg-pink-100 text-pink-700',
}

export function ChecklistTemplateDetailPage() {
  const { id } = useParams({ from: '/checklist-templates/$id' })
  const navigate = useNavigate()
  const qc = useQueryClient()

  const [showSectionModal, setShowSectionModal] = useState(false)
  const [sectionForm, setSectionForm] = useState({ name: '' })

  const [showItemModal, setShowItemModal] = useState<string | null>(null) // sectionId
  const [itemForm, setItemForm] = useState({ question: '', itemType: 'YesNo', isRequired: true, helpText: '' })

  const [sectionError, setSectionError] = useState('')
  const [itemError, setItemError] = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['checklist-template', id],
    queryFn: () => api.get<TemplateDetail>(`/checklist-templates/${id}`).then(r => r.data),
  })

  const addSectionMutation = useMutation({
    mutationFn: (name: string) =>
      api.post(`/checklist-templates/${id}/sections`, { name, order: (data?.sections.length ?? 0) + 1 }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['checklist-template', id] })
      qc.invalidateQueries({ queryKey: ['checklist-templates'] })
      setShowSectionModal(false)
      setSectionForm({ name: '' })
      setSectionError('')
    },
    onError: (err: any) => setSectionError(err.response?.data?.message ?? 'Error'),
  })

  const addItemMutation = useMutation({
    mutationFn: ({ sectionId, data: d }: { sectionId: string; data: typeof itemForm }) => {
      const section = data?.sections.find(s => s.id === sectionId)
      const order = (section?.items.length ?? 0) + 1
      return api.post(`/checklist-templates/${id}/sections/${sectionId}/items`, {
        question: d.question,
        itemType: d.itemType,
        isRequired: d.isRequired,
        helpText: d.helpText.trim() || undefined,
        order,
      })
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['checklist-template', id] })
      setShowItemModal(null)
      setItemForm({ question: '', itemType: 'YesNo', isRequired: true, helpText: '' })
      setItemError('')
    },
    onError: (err: any) => setItemError(err.response?.data?.message ?? 'Error'),
  })

  function handleAddSection() {
    if (!sectionForm.name.trim()) { setSectionError('El nombre es requerido'); return }
    addSectionMutation.mutate(sectionForm.name.trim())
  }

  function handleAddItem(sectionId: string) {
    if (!itemForm.question.trim()) { setItemError('La pregunta es requerida'); return }
    addItemMutation.mutate({ sectionId, data: itemForm })
  }

  if (isLoading) return <div className="p-8 text-gray-400">Cargando...</div>
  if (!data) return <div className="p-8 text-gray-400">Plantilla no encontrada</div>

  return (
    <div className="p-8 max-w-3xl">
      <button
        onClick={() => navigate({ to: '/checklist-templates' })}
        className="flex items-center gap-1 text-sm text-blue-600 hover:underline mb-4"
      >
        <ChevronLeft className="h-4 w-4" /> Volver a plantillas
      </button>

      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900">{data.name}</h1>
        {data.description && <p className="text-sm text-gray-500 mt-1">{data.description}</p>}
        <p className="text-xs text-gray-400 mt-1">Versión {data.version} · {data.sections.length} sección{data.sections.length !== 1 ? 'es' : ''}</p>
      </div>

      <div className="space-y-4">
        {data.sections.map(section => (
          <div key={section.id} className="rounded-xl border border-gray-200 bg-white overflow-hidden">
            <div className="flex items-center justify-between bg-gray-50 px-4 py-3 border-b border-gray-200">
              <div>
                <span className="text-xs font-semibold text-gray-400 uppercase tracking-wide mr-2">Sección {section.order}</span>
                <span className="font-semibold text-gray-800">{section.name}</span>
              </div>
              <button
                onClick={() => { setShowItemModal(section.id); setItemError('') }}
                className="flex items-center gap-1 text-xs font-semibold text-blue-600 hover:text-blue-800"
              >
                <Plus className="h-3.5 w-3.5" /> Agregar ítem
              </button>
            </div>

            {section.items.length === 0 ? (
              <div className="px-4 py-4 text-sm text-gray-400 italic">Sin ítems aún</div>
            ) : (
              <div className="divide-y divide-gray-100">
                {section.items.map(item => (
                  <div key={item.id} className="flex items-start gap-3 px-4 py-3">
                    <span className="mt-0.5 text-sm font-medium text-gray-400 w-5 shrink-0">{item.order}.</span>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-gray-800">{item.question}</p>
                      {item.helpText && <p className="text-xs text-gray-400 mt-0.5">{item.helpText}</p>}
                    </div>
                    <div className="flex items-center gap-2 shrink-0">
                      <span className={`text-xs font-semibold px-2 py-0.5 rounded-full ${itemTypeColor[item.itemType] ?? 'bg-gray-100 text-gray-600'}`}>
                        {itemTypeLabel[item.itemType] ?? item.itemType}
                      </span>
                      {item.isRequired && (
                        <span className="text-xs text-red-500 font-medium">*</span>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        ))}

        <button
          onClick={() => { setShowSectionModal(true); setSectionError('') }}
          className="w-full flex items-center justify-center gap-2 rounded-xl border-2 border-dashed border-gray-300 py-3 text-sm font-semibold text-gray-500 hover:border-blue-400 hover:text-blue-600 transition-colors"
        >
          <Plus className="h-4 w-4" /> Agregar sección
        </button>
      </div>

      {/* Modal nueva sección */}
      {showSectionModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="w-full max-w-sm rounded-2xl bg-white p-6 shadow-xl">
            <h2 className="text-lg font-bold text-gray-900 mb-4">Nueva sección</h2>
            {sectionError && <p className="mb-3 rounded-lg bg-red-50 px-3 py-2 text-sm text-red-600">{sectionError}</p>}
            <label className="block text-sm font-medium text-gray-700 mb-1">Nombre de la sección *</label>
            <input
              className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
              value={sectionForm.name}
              onChange={e => setSectionForm({ name: e.target.value })}
              placeholder="Ej: Instalación interna"
              autoFocus
            />
            <div className="mt-4 flex justify-end gap-3">
              <button onClick={() => { setShowSectionModal(false); setSectionError('') }} className="rounded-lg px-4 py-2 text-sm font-medium text-gray-600 hover:bg-gray-100">
                Cancelar
              </button>
              <button
                onClick={handleAddSection}
                disabled={addSectionMutation.isPending}
                className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700 disabled:opacity-60"
              >
                {addSectionMutation.isPending ? 'Guardando...' : 'Agregar'}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Modal nuevo ítem */}
      {showItemModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="w-full max-w-md rounded-2xl bg-white p-6 shadow-xl">
            <h2 className="text-lg font-bold text-gray-900 mb-4">Nuevo ítem</h2>
            {itemError && <p className="mb-3 rounded-lg bg-red-50 px-3 py-2 text-sm text-red-600">{itemError}</p>}
            <div className="space-y-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Pregunta *</label>
                <input
                  className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
                  value={itemForm.question}
                  onChange={e => setItemForm(f => ({ ...f, question: e.target.value }))}
                  placeholder="Ej: ¿La válvula de paso está en buen estado?"
                  autoFocus
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Tipo de respuesta</label>
                <select
                  className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
                  value={itemForm.itemType}
                  onChange={e => setItemForm(f => ({ ...f, itemType: e.target.value }))}
                >
                  <option value="YesNo">Sí / No</option>
                  <option value="Text">Texto libre</option>
                  <option value="Numeric">Numérico</option>
                  <option value="Photo">Foto</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Texto de ayuda (opcional)</label>
                <input
                  className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
                  value={itemForm.helpText}
                  onChange={e => setItemForm(f => ({ ...f, helpText: e.target.value }))}
                  placeholder="Indicación adicional para el técnico"
                />
              </div>
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="checkbox"
                  className="h-4 w-4 rounded"
                  checked={itemForm.isRequired}
                  onChange={e => setItemForm(f => ({ ...f, isRequired: e.target.checked }))}
                />
                <span className="text-sm text-gray-700">Ítem requerido</span>
              </label>
            </div>
            <div className="mt-5 flex justify-end gap-3">
              <button onClick={() => { setShowItemModal(null); setItemError('') }} className="rounded-lg px-4 py-2 text-sm font-medium text-gray-600 hover:bg-gray-100">
                Cancelar
              </button>
              <button
                onClick={() => handleAddItem(showItemModal)}
                disabled={addItemMutation.isPending}
                className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700 disabled:opacity-60"
              >
                {addItemMutation.isPending ? 'Guardando...' : 'Agregar ítem'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
