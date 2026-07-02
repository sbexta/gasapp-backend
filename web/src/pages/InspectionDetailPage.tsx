import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useParams, useNavigate } from '@tanstack/react-router'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { api } from '@/lib/api'
import type { InspectionDetailDto } from '@/types/api'

const statusLabel: Record<string, string> = {
  Pending: 'Pendiente', PreCheck: 'Pre-revisión', InProgress: 'En progreso',
  TechnicalReview: 'Revisión técnica', GeneratingDocs: 'Generando docs',
  Completed: 'Completada', RequiresFollowup: 'Requiere seguimiento', Rejected: 'Rechazada',
}

const statusVariant: Record<string, 'default' | 'success' | 'warning' | 'destructive' | 'secondary'> = {
  Pending: 'secondary', PreCheck: 'secondary', InProgress: 'default',
  TechnicalReview: 'warning', GeneratingDocs: 'warning', Completed: 'success',
  RequiresFollowup: 'destructive', Rejected: 'destructive',
}

const severityLabel: Record<string, string> = {
  Low: 'Baja', Medium: 'Media', High: 'Alta', Critical: 'Crítica',
}
const severityColor: Record<string, string> = {
  Low: 'text-green-700 bg-green-50', Medium: 'text-yellow-700 bg-yellow-50',
  High: 'text-orange-700 bg-orange-50', Critical: 'text-red-700 bg-red-50',
}

export function InspectionDetailPage() {
  const { id } = useParams({ from: '/inspections/$id' })
  const navigate = useNavigate()
  const qc = useQueryClient()
  const [supervisorNotes, setSupervisorNotes] = useState('')
  const [showApproveModal, setShowApproveModal] = useState(false)

  const { data, isLoading } = useQuery({
    queryKey: ['inspection', id],
    queryFn: () => api.get<InspectionDetailDto>(`/inspections/${id}`).then((r) => r.data),
  })

  const approveMutation = useMutation({
    mutationFn: () =>
      api.post(`/inspections/${id}/approve`, { supervisorNotes: supervisorNotes.trim() || null }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['inspection', id] })
      qc.invalidateQueries({ queryKey: ['inspections'] })
      setShowApproveModal(false)
      setSupervisorNotes('')
    },
  })

  if (isLoading) {
    return <div className="p-8 text-gray-400">Cargando...</div>
  }

  if (!data) {
    return <div className="p-8 text-gray-400">Inspección no encontrada</div>
  }

  return (
    <div className="p-8 max-w-4xl">
      <div className="mb-6 flex items-start justify-between">
        <div>
          <button
            onClick={() => navigate({ to: '/inspections' })}
            className="mb-2 text-sm text-blue-600 hover:underline"
          >
            ← Volver a inspecciones
          </button>
          <h1 className="text-2xl font-bold text-gray-900">Orden {data.orderNumber}</h1>
          <p className="text-sm text-gray-500">
            Programada: {new Date(data.scheduledDate).toLocaleDateString('es-CO', { day: 'numeric', month: 'long', year: 'numeric' })}
          </p>
        </div>
        <div className="flex items-center gap-3">
          <Badge variant={statusVariant[data.status] ?? 'secondary'}>
            {statusLabel[data.status] ?? data.status}
          </Badge>
          {data.status === 'TechnicalReview' && (
            <Button onClick={() => setShowApproveModal(true)}>Aprobar inspección</Button>
          )}
        </div>
      </div>

      {/* Info general */}
      <div className="mb-6 grid grid-cols-3 gap-4">
        <div className="rounded-lg border border-gray-200 bg-white p-4">
          <p className="text-xs text-gray-500">Iniciada</p>
          <p className="text-sm font-medium text-gray-900">
            {data.startedAt ? new Date(data.startedAt).toLocaleString('es-CO') : '—'}
          </p>
        </div>
        <div className="rounded-lg border border-gray-200 bg-white p-4">
          <p className="text-xs text-gray-500">Completada</p>
          <p className="text-sm font-medium text-gray-900">
            {data.completedAt ? new Date(data.completedAt).toLocaleString('es-CO') : '—'}
          </p>
        </div>
        <div className="rounded-lg border border-gray-200 bg-white p-4">
          <p className="text-xs text-gray-500">Firma</p>
          <p className={`text-sm font-medium ${data.hasSignature ? 'text-green-700' : 'text-gray-400'}`}>
            {data.hasSignature ? '✓ Registrada' : 'Sin firma'}
          </p>
        </div>
      </div>

      {data.technicianNotes && (
        <div className="mb-6 rounded-lg border border-gray-200 bg-white p-4">
          <p className="mb-1 text-xs font-medium text-gray-500">Notas del técnico</p>
          <p className="text-sm text-gray-700">{data.technicianNotes}</p>
        </div>
      )}

      {/* Hallazgos */}
      <div className="mb-6">
        <h2 className="mb-3 text-base font-semibold text-gray-900">
          Hallazgos ({data.findings.length})
        </h2>
        {data.findings.length === 0 ? (
          <p className="text-sm text-gray-400">Sin hallazgos registrados</p>
        ) : (
          <div className="space-y-2">
            {data.findings.map((f) => (
              <div key={f.id} className="rounded-lg border border-gray-200 bg-white p-4">
                <div className="mb-1 flex items-start justify-between gap-2">
                  <p className="text-sm font-medium text-gray-900">{f.description}</p>
                  <span className={`shrink-0 rounded-full px-2 py-0.5 text-xs font-semibold ${severityColor[f.severity] ?? ''}`}>
                    {severityLabel[f.severity] ?? f.severity}
                  </span>
                </div>
                {f.correctiveAction && (
                  <p className="text-xs text-gray-500">Acción correctiva: {f.correctiveAction}</p>
                )}
                {f.requiresCorrection && (
                  <p className={`mt-1 text-xs font-medium ${f.isResolved ? 'text-green-600' : 'text-orange-600'}`}>
                    {f.isResolved ? '✓ Corregido' : '⚠ Requiere corrección'}
                  </p>
                )}
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Respuestas del checklist */}
      <div>
        <h2 className="mb-3 text-base font-semibold text-gray-900">
          Respuestas del checklist ({data.responses.length})
        </h2>
        {data.responses.length === 0 ? (
          <p className="text-sm text-gray-400">Sin respuestas registradas</p>
        ) : (
          <div className="rounded-lg border border-gray-200 bg-white">
            <table className="w-full text-sm">
              <thead className="border-b border-gray-200 bg-gray-50">
                <tr>
                  <th className="px-4 py-3 text-left font-medium text-gray-600">Ítem</th>
                  <th className="px-4 py-3 text-left font-medium text-gray-600">Respuesta</th>
                  <th className="px-4 py-3 text-left font-medium text-gray-600">Cumple</th>
                  <th className="px-4 py-3 text-left font-medium text-gray-600">Notas</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {data.responses.map((r) => (
                  <tr key={r.id}>
                    <td className="px-4 py-3 text-xs text-gray-500 font-mono">{r.checklistItemId.slice(0, 8)}…</td>
                    <td className="px-4 py-3 text-gray-700">
                      {r.boolValue !== null
                        ? (r.boolValue ? 'Sí' : 'No')
                        : r.numericValue !== null
                        ? r.numericValue
                        : r.textValue ?? '—'}
                    </td>
                    <td className="px-4 py-3">
                      <span className={`font-medium ${r.complies ? 'text-green-600' : 'text-red-600'}`}>
                        {r.complies ? '✓' : '✗'}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-gray-500">{r.notes ?? '—'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Modal aprobar */}
      {showApproveModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="w-full max-w-md rounded-xl bg-white p-6 shadow-xl">
            <h2 className="mb-4 text-lg font-semibold text-gray-900">Aprobar inspección</h2>
            <p className="mb-4 text-sm text-gray-500">
              Esta acción completará la inspección. Opcionalmente puedes añadir notas de revisión.
            </p>
            <textarea
              value={supervisorNotes}
              onChange={(e) => setSupervisorNotes(e.target.value)}
              placeholder="Notas del supervisor (opcional)..."
              rows={3}
              className="w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
            />
            {approveMutation.isError && (
              <p className="mt-2 text-sm text-red-500">Error al aprobar. Intenta de nuevo.</p>
            )}
            <div className="mt-4 flex justify-end gap-3">
              <Button variant="outline" onClick={() => setShowApproveModal(false)}>
                Cancelar
              </Button>
              <Button onClick={() => approveMutation.mutate()} disabled={approveMutation.isPending}>
                {approveMutation.isPending ? 'Aprobando...' : 'Confirmar aprobación'}
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
