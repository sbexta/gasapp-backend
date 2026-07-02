import { View, Text, ScrollView, TouchableOpacity, StyleSheet, Alert, ActivityIndicator } from 'react-native'
import { useLocalSearchParams, useRouter } from 'expo-router'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { SafeAreaView } from 'react-native-safe-area-context'
import { api } from '@/lib/api'
import { ConnectivityBanner } from '@/components/ConnectivityBanner'
import type { WorkOrderDetailDto } from '@/types/api'

const statusLabel: Record<string, string> = {
  Draft: 'Borrador', Scheduled: 'Programada', Assigned: 'Asignada',
  InProgress: 'En progreso', Completed: 'Completada', Cancelled: 'Cancelada',
}
const statusColor: Record<string, string> = {
  Draft: '#9CA3AF', Scheduled: '#3B82F6', Assigned: '#F59E0B',
  InProgress: '#8B5CF6', Completed: '#10B981', Cancelled: '#EF4444',
}

export default function WorkOrderDetailScreen() {
  const { id } = useLocalSearchParams<{ id: string }>()
  const router = useRouter()
  const queryClient = useQueryClient()

  const { data, isLoading } = useQuery({
    queryKey: ['work-order', id],
    queryFn: () => api.get<WorkOrderDetailDto>(`/work-orders/${id}`).then((r) => r.data),
    enabled: !!id,
  })

  const startMutation = useMutation({
    mutationFn: () => api.post(`/work-orders/${id}/start`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['work-order', id] })
      queryClient.invalidateQueries({ queryKey: ['agenda'] })
      queryClient.invalidateQueries({ queryKey: ['work-orders-all'] })
      Alert.alert('Listo', 'Inspección iniciada correctamente')
    },
    onError: () => Alert.alert('Error', 'No se pudo iniciar la inspección'),
  })

  function handleStart() {
    Alert.alert(
      'Iniciar inspección',
      '¿Confirmas que vas a iniciar esta orden de trabajo?',
      [
        { text: 'Cancelar', style: 'cancel' },
        { text: 'Iniciar', onPress: () => startMutation.mutate() },
      ]
    )
  }

  if (isLoading) {
    return (
      <SafeAreaView style={styles.center}>
        <ActivityIndicator size="large" color="#1D4ED8" />
      </SafeAreaView>
    )
  }

  if (!data) {
    return (
      <SafeAreaView style={styles.center}>
        <Text style={styles.errorText}>No se encontró la orden</Text>
      </SafeAreaView>
    )
  }

  const canStart = data.status === 'Assigned'

  return (
    <SafeAreaView style={styles.container} edges={['top']}>
      <ConnectivityBanner />

      <View style={styles.topBar}>
        <TouchableOpacity onPress={() => router.back()} style={styles.backBtn}>
          <Text style={styles.backText}>← Volver</Text>
        </TouchableOpacity>
      </View>

      <ScrollView contentContainerStyle={styles.scroll}>
        <View style={styles.titleRow}>
          <Text style={styles.orderNumber}>{data.orderNumber}</Text>
          <View style={[styles.badge, { backgroundColor: statusColor[data.status] + '20' }]}>
            <Text style={[styles.badgeText, { color: statusColor[data.status] }]}>
              {statusLabel[data.status] ?? data.status}
            </Text>
          </View>
        </View>

        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Cliente</Text>
          <Text style={styles.value}>{data.client.businessName}</Text>
          {data.client.contactPhone && (
            <Text style={styles.meta}>{data.client.contactPhone}</Text>
          )}
        </View>

        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Sede</Text>
          <Text style={styles.value}>{data.location.name}</Text>
          <Text style={styles.meta}>{data.location.address}</Text>
          <Text style={styles.meta}>{data.location.municipality}, {data.location.department}</Text>
        </View>

        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Fecha programada</Text>
          <Text style={styles.value}>
            {new Date(data.scheduledDate).toLocaleDateString('es-CO', {
              weekday: 'long', day: 'numeric', month: 'long', year: 'numeric',
            })}
          </Text>
        </View>

        {data.notes && (
          <View style={styles.section}>
            <Text style={styles.sectionTitle}>Notas</Text>
            <Text style={styles.value}>{data.notes}</Text>
          </View>
        )}
      </ScrollView>

      <View style={styles.footer}>
        {canStart && (
          <TouchableOpacity
            style={[styles.startButton, startMutation.isPending && styles.startButtonDisabled]}
            onPress={handleStart}
            disabled={startMutation.isPending}
          >
            {startMutation.isPending
              ? <ActivityIndicator color="#fff" />
              : <Text style={styles.startButtonText}>Iniciar inspección</Text>
            }
          </TouchableOpacity>
        )}
        {data.status === 'InProgress' && (
          <>
            <View style={styles.actionRow}>
              <TouchableOpacity
                style={styles.actionBtn}
                onPress={() => router.push(`/(app)/checklist/${id}`)}
              >
                <Text style={styles.actionBtnText}>📋 Checklist</Text>
              </TouchableOpacity>
              <TouchableOpacity
                style={styles.actionBtn}
                onPress={() => {
                  if (data.inspectionId) {
                    router.push(`/(app)/findings/${data.inspectionId}`)
                  }
                }}
              >
                <Text style={styles.actionBtnText}>⚠️ Hallazgos</Text>
              </TouchableOpacity>
            </View>
            <TouchableOpacity
              style={styles.signatureBtn}
              onPress={() => {
                if (data.inspectionId) {
                  router.push(`/(app)/signature/${data.inspectionId}`)
                }
              }}
            >
              <Text style={styles.signatureBtnText}>✍️ Firmar y finalizar inspección</Text>
            </TouchableOpacity>
          </>
        )}
      </View>
    </SafeAreaView>
  )
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#F9FAFB' },
  center: { flex: 1, alignItems: 'center', justifyContent: 'center' },
  errorText: { color: '#9CA3AF', fontSize: 15 },
  topBar: {
    backgroundColor: '#fff',
    paddingHorizontal: 20,
    paddingVertical: 12,
    borderBottomWidth: 1,
    borderBottomColor: '#E5E7EB',
  },
  backBtn: { alignSelf: 'flex-start' },
  backText: { fontSize: 15, color: '#1D4ED8', fontWeight: '500' },
  scroll: { padding: 20, gap: 4 },
  titleRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 20,
  },
  orderNumber: { fontSize: 22, fontWeight: '700', color: '#111827' },
  badge: { paddingHorizontal: 10, paddingVertical: 4, borderRadius: 99 },
  badgeText: { fontSize: 12, fontWeight: '600' },
  section: {
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 16,
    marginBottom: 12,
    elevation: 1,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.04,
    shadowRadius: 3,
  },
  sectionTitle: { fontSize: 11, fontWeight: '600', color: '#9CA3AF', textTransform: 'uppercase', letterSpacing: 0.5, marginBottom: 6 },
  value: { fontSize: 15, fontWeight: '500', color: '#111827', marginBottom: 2 },
  meta: { fontSize: 13, color: '#6B7280' },
  footer: {
    backgroundColor: '#fff',
    padding: 16,
    borderTopWidth: 1,
    borderTopColor: '#E5E7EB',
    gap: 10,
  },
  actionRow: { flexDirection: 'row', gap: 10 },
  actionBtn: {
    flex: 1, height: 44, borderRadius: 10, backgroundColor: '#EFF6FF',
    alignItems: 'center', justifyContent: 'center', borderWidth: 1, borderColor: '#BFDBFE',
  },
  actionBtnText: { color: '#1D4ED8', fontWeight: '600', fontSize: 14 },
  signatureBtn: {
    height: 50, borderRadius: 12, backgroundColor: '#059669',
    alignItems: 'center', justifyContent: 'center',
  },
  signatureBtnText: { color: '#fff', fontWeight: '700', fontSize: 15 },
  startButton: {
    height: 50,
    backgroundColor: '#1D4ED8',
    borderRadius: 12,
    alignItems: 'center',
    justifyContent: 'center',
  },
  startButtonDisabled: { opacity: 0.6 },
  startButtonText: { color: '#fff', fontWeight: '700', fontSize: 16 },
})
