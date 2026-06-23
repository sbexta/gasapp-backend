import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Badge } from '@/components/ui/badge'
import { api } from '@/lib/api'
import type { PagedResult, WorkOrderDto } from '@/types/api'

const statusLabel: Record<string, string> = {
  Draft: 'Borrador', Scheduled: 'Programada', Assigned: 'Asignada',
  InProgress: 'En progreso', Completed: 'Completada', Cancelled: 'Cancelada',
}
const statusVariant: Record<string, 'default' | 'success' | 'warning' | 'destructive' | 'secondary'> = {
  Draft: 'secondary', Scheduled: 'default', Assigned: 'warning',
  InProgress: 'default', Completed: 'success', Cancelled: 'destructive',
}

export function WorkOrdersPage() {
  const [statusFilter, setStatusFilter] = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['work-orders', statusFilter],
    queryFn: () =>
      api.get<PagedResult<WorkOrderDto>>(
        `/work-orders?pageSize=50${statusFilter ? `&status=${statusFilter}` : ''}`
      ).then((r) => r.data),
  })

  return (
    <div className="p-8">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Órdenes de trabajo</h1>
        <p className="text-sm text-gray-500">{data?.totalCount ?? 0} órdenes registradas</p>
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
              <th className="px-4 py-3 text-left font-medium text-gray-600">Técnico asignado</th>
              <th className="px-4 py-3 text-left font-medium text-gray-600">Estado</th>
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
                <td className="px-4 py-3 text-gray-600">
                  {o.assignedTechnicianId ? o.assignedTechnicianId.slice(0, 8) + '...' : '—'}
                </td>
                <td className="px-4 py-3">
                  <Badge variant={statusVariant[o.status] ?? 'secondary'}>
                    {statusLabel[o.status] ?? o.status}
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
