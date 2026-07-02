import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { Badge } from '@/components/ui/badge'

interface ReportRow {
  inspectionId: string
  orderNumber: string
  status: string
  scheduledDate: string
  completedAt: string | null
  technicianName: string
  clientName: string
  locationName: string
  findingsCount: number
  hasCertificate: boolean
}

interface ReportResult { items: ReportRow[]; total: number }

interface Kpis {
  total: number; completed: number; inProgress: number
  inReview: number; rejected: number; completionRate: number
  avgCompletionDays: number | null
}

const statusLabel: Record<string, string> = {
  Pending: 'Pendiente', PreCheck: 'Pre-revisión', InProgress: 'En progreso',
  TechnicalReview: 'Revisión técnica', GeneratingDocs: 'Generando docs',
  Completed: 'Completada', RequiresFollowup: 'Seguimiento', Rejected: 'Rechazada',
}
const statusVariant: Record<string, 'default' | 'success' | 'warning' | 'destructive' | 'secondary'> = {
  Completed: 'success', InProgress: 'default', TechnicalReview: 'warning',
  Rejected: 'destructive', Pending: 'secondary',
}

const STATUSES = ['', 'InProgress', 'TechnicalReview', 'Completed', 'Rejected']

export function ReportsPage() {
  const [from, setFrom] = useState('')
  const [to, setTo] = useState('')
  const [status, setStatus] = useState('')
  const [page, setPage] = useState(1)
  const pageSize = 25

  const params = new URLSearchParams()
  if (from) params.set('from', from)
  if (to) params.set('to', to)
  if (status) params.set('status', status)
  params.set('page', String(page))
  params.set('pageSize', String(pageSize))

  const kpiParams = new URLSearchParams()
  if (from) kpiParams.set('from', from)
  if (to) kpiParams.set('to', to)

  const { data, isLoading } = useQuery({
    queryKey: ['reports', 'inspections', from, to, status, page],
    queryFn: () => api.get<ReportResult>(`/reports/inspections?${params}`).then(r => r.data),
  })

  const { data: kpis } = useQuery({
    queryKey: ['reports', 'kpis', from, to],
    queryFn: () => api.get<Kpis>(`/reports/kpis?${kpiParams}`).then(r => r.data),
    refetchInterval: 30_000,
  })

  const totalPages = Math.ceil((data?.total ?? 0) / pageSize)

  function handleExport() {
    const ep = new URLSearchParams()
    if (from) ep.set('from', from)
    if (to) ep.set('to', to)
    if (status) ep.set('status', status)
    window.open(`/api/v1/reports/inspections/export?${ep}`, '_blank')
  }

  function handleFilter() { setPage(1) }

  return (
    <div className="p-8">
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Reportes</h1>
          <p className="text-sm text-gray-500">Análisis operativo de inspecciones</p>
        </div>
        <button
          onClick={handleExport}
          className="flex items-center gap-2 rounded-lg border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
        >
          ↓ Exportar CSV
        </button>
      </div>

      {/* KPIs */}
      {kpis && (
        <div className="mb-6 grid grid-cols-2 gap-3 sm:grid-cols-4 lg:grid-cols-7">
          {[
            { label: 'Total', value: kpis.total, color: 'text-gray-900' },
            { label: 'Completadas', value: kpis.completed, color: 'text-green-700' },
            { label: 'En progreso', value: kpis.inProgress, color: 'text-blue-700' },
            { label: 'En revisión', value: kpis.inReview, color: 'text-yellow-700' },
            { label: 'Rechazadas', value: kpis.rejected, color: 'text-red-700' },
            { label: 'Tasa completadas', value: `${kpis.completionRate}%`, color: 'text-purple-700' },
            { label: 'Días promedio', value: kpis.avgCompletionDays != null ? `${kpis.avgCompletionDays}d` : '—', color: 'text-gray-600' },
          ].map(({ label, value, color }) => (
            <div key={label} className="rounded-xl border border-gray-200 bg-white p-4">
              <p className={`text-2xl font-bold ${color}`}>{value}</p>
              <p className="text-xs text-gray-500">{label}</p>
            </div>
          ))}
        </div>
      )}

      {/* Filtros */}
      <div className="mb-4 flex flex-wrap gap-3">
        <div className="flex items-center gap-2">
          <label className="text-sm font-medium text-gray-600">Desde</label>
          <input type="date" value={from} onChange={e => setFrom(e.target.value)}
            className="rounded-lg border border-gray-300 px-3 py-1.5 text-sm focus:border-blue-500 focus:outline-none" />
        </div>
        <div className="flex items-center gap-2">
          <label className="text-sm font-medium text-gray-600">Hasta</label>
          <input type="date" value={to} onChange={e => setTo(e.target.value)}
            className="rounded-lg border border-gray-300 px-3 py-1.5 text-sm focus:border-blue-500 focus:outline-none" />
        </div>
        <select value={status} onChange={e => setStatus(e.target.value)}
          className="rounded-lg border border-gray-300 px-3 py-1.5 text-sm focus:border-blue-500 focus:outline-none">
          {STATUSES.map(s => <option key={s} value={s}>{s ? statusLabel[s] ?? s : 'Todos los estados'}</option>)}
        </select>
        <button onClick={handleFilter}
          className="rounded-lg bg-blue-600 px-4 py-1.5 text-sm font-semibold text-white hover:bg-blue-700">
          Filtrar
        </button>
        {(from || to || status) && (
          <button onClick={() => { setFrom(''); setTo(''); setStatus(''); setPage(1) }}
            className="rounded-lg border border-gray-300 px-4 py-1.5 text-sm text-gray-600 hover:bg-gray-50">
            Limpiar
          </button>
        )}
      </div>

      {/* Tabla */}
      <div className="rounded-xl border border-gray-200 bg-white overflow-hidden">
        {isLoading ? (
          <div className="p-8 text-center text-gray-400">Cargando...</div>
        ) : (
          <>
            <table className="w-full text-sm">
              <thead className="border-b border-gray-200 bg-gray-50">
                <tr>
                  {['Orden', 'Estado', 'Fecha prog.', 'Completada', 'Técnico', 'Cliente', 'Sede', 'Hallazgos', 'Cert.'].map(h => (
                    <th key={h} className="px-4 py-3 text-left font-medium text-gray-600">{h}</th>
                  ))}
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {(data?.items ?? []).map(row => (
                  <tr key={row.inspectionId} className="hover:bg-gray-50">
                    <td className="px-4 py-3 font-medium text-gray-900">{row.orderNumber}</td>
                    <td className="px-4 py-3">
                      <Badge variant={statusVariant[row.status] ?? 'secondary'}>
                        {statusLabel[row.status] ?? row.status}
                      </Badge>
                    </td>
                    <td className="px-4 py-3 text-gray-600">
                      {new Date(row.scheduledDate).toLocaleDateString('es-CO')}
                    </td>
                    <td className="px-4 py-3 text-gray-600">
                      {row.completedAt ? new Date(row.completedAt).toLocaleDateString('es-CO') : '—'}
                    </td>
                    <td className="px-4 py-3 text-gray-700">{row.technicianName}</td>
                    <td className="px-4 py-3 text-gray-700">{row.clientName}</td>
                    <td className="px-4 py-3 text-gray-700">{row.locationName}</td>
                    <td className="px-4 py-3 text-center">
                      {row.findingsCount > 0
                        ? <span className="text-orange-600 font-medium">{row.findingsCount}</span>
                        : <span className="text-gray-400">0</span>}
                    </td>
                    <td className="px-4 py-3 text-center">
                      {row.hasCertificate
                        ? <a href={`/api/v1/inspections/${row.inspectionId}/certificate`} target="_blank" rel="noreferrer"
                            className="text-green-600 font-medium hover:underline">↓ PDF</a>
                        : <span className="text-gray-400">—</span>}
                    </td>
                  </tr>
                ))}
                {data?.items.length === 0 && (
                  <tr><td colSpan={9} className="px-4 py-8 text-center text-gray-400">Sin resultados</td></tr>
                )}
              </tbody>
            </table>

            {totalPages > 1 && (
              <div className="flex items-center justify-between border-t border-gray-200 px-4 py-3">
                <p className="text-sm text-gray-500">{data?.total} resultados · Página {page} de {totalPages}</p>
                <div className="flex gap-2">
                  <button onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}
                    className="rounded-lg border px-3 py-1 text-sm disabled:opacity-40">Anterior</button>
                  <button onClick={() => setPage(p => Math.min(totalPages, p + 1))} disabled={page === totalPages}
                    className="rounded-lg border px-3 py-1 text-sm disabled:opacity-40">Siguiente</button>
                </div>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  )
}
