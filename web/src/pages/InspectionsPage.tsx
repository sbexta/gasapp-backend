import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { useNavigate } from '@tanstack/react-router'
import { Badge } from '@/components/ui/badge'
import { api } from '@/lib/api'
import type { PagedResult, InspectionListDto } from '@/types/api'

const statusLabel: Record<string, string> = {
  Pending: 'Pendiente',
  PreCheck: 'Pre-revisión',
  InProgress: 'En progreso',
  TechnicalReview: 'Revisión técnica',
  GeneratingDocs: 'Generando docs',
  Completed: 'Completada',
  RequiresFollowup: 'Requiere seguimiento',
  Rejected: 'Rechazada',
}

const statusVariant: Record<string, 'default' | 'success' | 'warning' | 'destructive' | 'secondary'> = {
  Pending: 'secondary',
  PreCheck: 'secondary',
  InProgress: 'default',
  TechnicalReview: 'warning',
  GeneratingDocs: 'warning',
  Completed: 'success',
  RequiresFollowup: 'destructive',
  Rejected: 'destructive',
}

export function InspectionsPage() {
  const navigate = useNavigate()
  const [statusFilter, setStatusFilter] = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['inspections', statusFilter],
    queryFn: () =>
      api.get<PagedResult<InspectionListDto>>(
        `/inspections?pageSize=50${statusFilter ? `&status=${statusFilter}` : ''}`
      ).then((r) => r.data),
  })

  return (
    <div className="p-8">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Inspecciones</h1>
        <p className="text-sm text-gray-500">{data?.totalCount ?? 0} inspecciones registradas</p>
      </div>

      <div className="mb-4">
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
              <th className="px-4 py-3 text-left font-medium text-gray-600">Iniciada</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Completada</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Estado</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Acciones</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {isLoading ? (
              <tr><td colSpan={6} className="py-8 text-center text-gray-400">Cargando...</td></tr>
            ) : !data?.items.length ? (
              <tr><td colSpan={6} className="py-8 text-center text-gray-400">No hay inspecciones registradas</td></tr>
            ) : data.items.map((i) => (
              <tr key={i.id} className="hover:bg-gray-50">
                <td className="px-4 py-3 font-medium text-gray-900">{i.orderNumber}</td>
                <td className="px-4 py-3 text-gray-600">
                  {new Date(i.scheduledDate).toLocaleDateString('es-CO')}
                </td>
                <td className="px-4 py-3 text-gray-600">
                  {i.startedAt ? new Date(i.startedAt).toLocaleDateString('es-CO') : '—'}
                </td>
                <td className="px-4 py-3 text-gray-600">
                  {i.completedAt ? new Date(i.completedAt).toLocaleDateString('es-CO') : '—'}
                </td>
                <td className="px-4 py-3">
                  <Badge variant={statusVariant[i.status] ?? 'secondary'}>
                    {statusLabel[i.status] ?? i.status}
                  </Badge>
                </td>
                <td className="px-4 py-3">
                  <button
                    onClick={() => navigate({ to: '/inspections/$id', params: { id: i.id } })}
                    className="text-xs font-medium text-blue-600 hover:underline"
                  >
                    Ver detalle
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
