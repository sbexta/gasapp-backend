import { View, Text, FlatList, TouchableOpacity, StyleSheet, RefreshControl } from 'react-native'
import { useQuery } from '@tanstack/react-query'
import { useRouter } from 'expo-router'
import { SafeAreaView } from 'react-native-safe-area-context'
import { useAuthStore } from '@/store/auth'
import { api } from '@/lib/api'
import { ConnectivityBanner } from '@/components/ConnectivityBanner'
import type { AgendaItemDto } from '@/types/api'

const statusLabel: Record<string, string> = {
  Draft: 'Borrador',
  Scheduled: 'Programada',
  Assigned: 'Asignada',
  InProgress: 'En progreso',
  Completed: 'Completada',
  Cancelled: 'Cancelada',
}

const statusColor: Record<string, string> = {
  Draft: '#9CA3AF',
  Scheduled: '#3B82F6',
  Assigned: '#F59E0B',
  InProgress: '#8B5CF6',
  Completed: '#10B981',
  Cancelled: '#EF4444',
}

export default function AgendaScreen() {
  const { user, clearAuth } = useAuthStore()
  const router = useRouter()
  const today = new Date().toISOString().split('T')[0]

  const { data, isLoading, refetch } = useQuery({
    queryKey: ['agenda', today],
    queryFn: () =>
      api.get<AgendaItemDto[]>(`/work-orders/agenda?date=${today}`).then((r) => r.data),
  })

  function handleLogout() {
    clearAuth()
  }

  function renderItem({ item }: { item: AgendaItemDto }) {
    return (
      <TouchableOpacity
        style={styles.card}
        onPress={() => router.push(`/(app)/work-order/${item.workOrderId}`)}
        activeOpacity={0.7}
      >
        <View style={styles.cardHeader}>
          <Text style={styles.orderNumber}>{item.orderNumber}</Text>
          <View style={[styles.badge, { backgroundColor: statusColor[item.status] + '20' }]}>
            <Text style={[styles.badgeText, { color: statusColor[item.status] }]}>
              {statusLabel[item.status] ?? item.status}
            </Text>
          </View>
        </View>
        <Text style={styles.clientName}>{item.clientName}</Text>
        <Text style={styles.locationName}>{item.locationName}</Text>
        <Text style={styles.address}>{item.locationAddress}, {item.municipality}</Text>
      </TouchableOpacity>
    )
  }

  return (
    <SafeAreaView style={styles.container} edges={['top']}>
      <ConnectivityBanner />
      <View style={styles.header}>
        <View>
          <Text style={styles.greeting}>Hola, {user?.fullName?.split(' ')[0]}</Text>
          <Text style={styles.dateText}>
            {new Date().toLocaleDateString('es-CO', { weekday: 'long', day: 'numeric', month: 'long' })}
          </Text>
        </View>
        <TouchableOpacity onPress={handleLogout} style={styles.logoutBtn}>
          <Text style={styles.logoutText}>Salir</Text>
        </TouchableOpacity>
      </View>

      <Text style={styles.sectionTitle}>
        Agenda de hoy — {data?.length ?? 0} orden{data?.length !== 1 ? 'es' : ''}
      </Text>

      <FlatList
        data={data ?? []}
        keyExtractor={(item) => item.workOrderId}
        renderItem={renderItem}
        contentContainerStyle={styles.list}
        refreshControl={<RefreshControl refreshing={isLoading} onRefresh={refetch} />}
        ListEmptyComponent={
          !isLoading ? (
            <View style={styles.empty}>
              <Text style={styles.emptyText}>No tienes órdenes para hoy</Text>
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
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    paddingHorizontal: 20,
    paddingTop: 16,
    paddingBottom: 8,
    backgroundColor: '#fff',
    borderBottomWidth: 1,
    borderBottomColor: '#E5E7EB',
  },
  greeting: { fontSize: 20, fontWeight: '700', color: '#111827' },
  dateText: { fontSize: 13, color: '#6B7280', marginTop: 2, textTransform: 'capitalize' },
  logoutBtn: { paddingVertical: 6, paddingHorizontal: 12 },
  logoutText: { fontSize: 13, color: '#6B7280' },
  sectionTitle: {
    fontSize: 13,
    fontWeight: '600',
    color: '#374151',
    paddingHorizontal: 20,
    paddingVertical: 12,
    textTransform: 'uppercase',
    letterSpacing: 0.5,
  },
  list: { padding: 16, gap: 12 },
  card: {
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },
  cardHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 6 },
  orderNumber: { fontSize: 15, fontWeight: '700', color: '#111827' },
  badge: { paddingHorizontal: 8, paddingVertical: 3, borderRadius: 99 },
  badgeText: { fontSize: 11, fontWeight: '600' },
  clientName: { fontSize: 14, fontWeight: '600', color: '#374151', marginBottom: 2 },
  locationName: { fontSize: 13, color: '#374151', marginBottom: 2 },
  address: { fontSize: 12, color: '#9CA3AF' },
  empty: { alignItems: 'center', paddingTop: 60 },
  emptyText: { fontSize: 15, color: '#9CA3AF' },
})
