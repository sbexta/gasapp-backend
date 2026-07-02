import { View, Text, FlatList, TouchableOpacity, StyleSheet, RefreshControl } from 'react-native'
import { useQuery } from '@tanstack/react-query'
import { useRouter } from 'expo-router'
import { SafeAreaView } from 'react-native-safe-area-context'
import { api } from '@/lib/api'
import { ConnectivityBanner } from '@/components/ConnectivityBanner'
import type { WorkOrderSummaryDto } from '@/types/api'

const statusLabel: Record<string, string> = {
  Draft: 'Borrador', Scheduled: 'Programada', Assigned: 'Asignada',
  InProgress: 'En progreso', Completed: 'Completada', Cancelled: 'Cancelada',
}
const statusColor: Record<string, string> = {
  Draft: '#9CA3AF', Scheduled: '#3B82F6', Assigned: '#F59E0B',
  InProgress: '#8B5CF6', Completed: '#10B981', Cancelled: '#EF4444',
}

export default function WorkOrdersScreen() {
  const router = useRouter()

  const { data, isLoading, refetch } = useQuery({
    queryKey: ['work-orders-all'],
    queryFn: () =>
      api.get<{ items: WorkOrderSummaryDto[] }>('/work-orders?pageSize=50').then((r) => r.data.items),
  })

  return (
    <SafeAreaView style={styles.container} edges={['top']}>
      <ConnectivityBanner />
      <View style={styles.header}>
        <Text style={styles.title}>Mis órdenes</Text>
      </View>

      <FlatList
        data={data ?? []}
        keyExtractor={(item) => item.id}
        contentContainerStyle={styles.list}
        refreshControl={<RefreshControl refreshing={isLoading} onRefresh={refetch} />}
        renderItem={({ item }) => (
          <TouchableOpacity
            style={styles.card}
            onPress={() => router.push(`/(app)/work-order/${item.id}`)}
            activeOpacity={0.7}
          >
            <View style={styles.row}>
              <Text style={styles.orderNumber}>{item.orderNumber}</Text>
              <View style={[styles.badge, { backgroundColor: statusColor[item.status] + '20' }]}>
                <Text style={[styles.badgeText, { color: statusColor[item.status] }]}>
                  {statusLabel[item.status] ?? item.status}
                </Text>
              </View>
            </View>
            <Text style={styles.date}>
              {new Date(item.scheduledDate).toLocaleDateString('es-CO', { day: 'numeric', month: 'short', year: 'numeric' })}
            </Text>
          </TouchableOpacity>
        )}
        ListEmptyComponent={
          !isLoading ? (
            <View style={styles.empty}>
              <Text style={styles.emptyText}>No hay órdenes asignadas</Text>
            </View>
          ) : null
        }
      />
    </SafeAreaView>
  )
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#F9FAFB' },
  header: {
    paddingHorizontal: 20,
    paddingVertical: 16,
    backgroundColor: '#fff',
    borderBottomWidth: 1,
    borderBottomColor: '#E5E7EB',
  },
  title: { fontSize: 20, fontWeight: '700', color: '#111827' },
  list: { padding: 16, gap: 12 },
  card: {
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 16,
    elevation: 2,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
  },
  row: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 4 },
  orderNumber: { fontSize: 15, fontWeight: '700', color: '#111827' },
  badge: { paddingHorizontal: 8, paddingVertical: 3, borderRadius: 99 },
  badgeText: { fontSize: 11, fontWeight: '600' },
  date: { fontSize: 12, color: '#9CA3AF' },
  empty: { alignItems: 'center', paddingTop: 60 },
  emptyText: { fontSize: 15, color: '#9CA3AF' },
})
