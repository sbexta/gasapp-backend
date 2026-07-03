import { useQuery } from '@tanstack/react-query'
import { Building2, ClipboardList, CheckCircle2, Clock } from 'lucide-react'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { api } from '@/lib/api'
import { useAuthStore } from '@/store/auth'
import type { PagedResult, WorkOrderDto } from '@/types/api'

const statusLabel: Record<string, string> = {
  Draft: 'Borrador', Scheduled: 'Programada', Assigned: 'Asignada',
  InProgress: 'En progreso', Completed: 'Completada', Cancelled: 'Cancelada',
}
const statusVariant: Record<string, 'default' | 'success' | 'warning' | 'destructive' | 'secondary'> = {
  Draft: 'secondary', Scheduled: 'default', Assigned: 'warning',
  InProgress: 'default', Completed: 'success', Cancelled: 'destructive',
}

export function DashboardPage() {
  const user = useAuthStore((s) => s.user)

  const { data: clientsData } = useQuery({
    queryKey: ['clients', 'count'],
    queryFn: () => api.get<PagedResult<unknown>>('/clients?pageSize=1').then((r) => r.data),
    refetchInterval: 30_000,
  })

  const { data: ordersData } = useQuery({
    queryKey: ['work-orders', 'recent'],
    queryFn: () => api.get<PagedResult<WorkOrderDto>>('/work-orders?pageSize=5').then((r) => r.data),
    refetchInterval: 30_000,
  })

  const stats = [
    { label: 'Clientes activos', value: clientsData?.total ?? '—', icon: Building2, color: 'text-blue-600' },
    { label: 'Órdenes totales', value: ordersData?.total ?? '—', icon: ClipboardList, color: 'text-purple-600' },
    {
      label: 'Completadas',
      value: ordersData?.items.filter((o) => o.status === 'Completed').length ?? '—',
      icon: CheckCircle2,
      color: 'text-green-600',
    },
    {
      label: 'En progreso',
      value: ordersData?.items.filter((o) => o.status === 'InProgress').length ?? '—',
      icon: Clock,
      color: 'text-yellow-600',
    },
  ]

  return (
    <div className="p-8">
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">Bienvenido, {user?.firstName}</h1>
        <p className="text-sm text-gray-500">Resumen del sistema de inspecciones</p>
      </div>

      <div className="mb-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {stats.map(({ label, value, icon: Icon, color }) => (
          <Card key={label}>
            <CardContent className="flex items-center gap-4 pt-6">
              <div className={`rounded-lg bg-gray-50 p-2 ${color}`}>
                <Icon className="h-5 w-5" />
              </div>
              <div>
                <p className="text-2xl font-bold text-gray-900">{value}</p>
                <p className="text-xs text-gray-500">{label}</p>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Órdenes recientes</CardTitle>
        </CardHeader>
        <CardContent>
          {!ordersData?.items.length ? (
            <p className="text-sm text-gray-500">No hay órdenes registradas aún.</p>
          ) : (
            <div className="divide-y divide-gray-100">
              {ordersData.items.map((order) => (
                <div key={order.id} className="flex items-center justify-between py-3">
                  <div>
                    <p className="text-sm font-medium text-gray-900">{order.orderNumber}</p>
                    <p className="text-xs text-gray-500">{new Date(order.scheduledDate).toLocaleDateString('es-CO')}</p>
                  </div>
                  <Badge variant={statusVariant[order.status] ?? 'secondary'}>
                    {statusLabel[order.status] ?? order.status}
                  </Badge>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
