import { useState, useEffect, useRef } from 'react'
import {
  View, Text, ScrollView, TouchableOpacity, TextInput,
  StyleSheet, Alert, ActivityIndicator,
} from 'react-native'
import { useLocalSearchParams, useRouter } from 'expo-router'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { SafeAreaView } from 'react-native-safe-area-context'
import * as Location from 'expo-location'
import { api } from '@/lib/api'
import { ConnectivityBanner } from '@/components/ConnectivityBanner'
import type { WorkOrderChecklistDto, ChecklistItemDto } from '@/types/api'

export default function ChecklistScreen() {
  const { workOrderId } = useLocalSearchParams<{ workOrderId: string }>()
  const router = useRouter()
  const queryClient = useQueryClient()
  const locationSent = useRef(false)

  const { data, isLoading } = useQuery({
    queryKey: ['checklist', workOrderId],
    queryFn: () =>
      api.get<WorkOrderChecklistDto>(`/inspections/by-work-order/${workOrderId}/checklist`)
        .then(r => r.data),
    enabled: !!workOrderId,
  })

  useEffect(() => {
    if (!data?.inspectionId || locationSent.current) return
    locationSent.current = true

    ;(async () => {
      const { status } = await Location.requestForegroundPermissionsAsync()
      if (status !== 'granted') return

      const loc = await Location.getCurrentPositionAsync({ accuracy: Location.Accuracy.High })
      await api.post(`/inspections/${data.inspectionId}/location`, {
        latitude: loc.coords.latitude,
        longitude: loc.coords.longitude,
      }).catch(() => {})
    })()
  }, [data?.inspectionId])

  const mutation = useMutation({
    mutationFn: (payload: { itemId: string; body: object }) =>
      api.post(`/inspections/${data?.inspectionId}/responses`, {
        checklistItemId: payload.itemId,
        ...payload.body,
      }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['checklist', workOrderId] }),
  })

  if (isLoading) {
    return <SafeAreaView style={styles.center}><ActivityIndicator size="large" color="#1D4ED8" /></SafeAreaView>
  }

  if (!data) {
    return (
      <SafeAreaView style={styles.center}>
        <Text style={styles.emptyText}>No hay checklist asignado a esta orden</Text>
      </SafeAreaView>
    )
  }

  const totalItems = data.sections.flatMap(s => s.items).length
  const answered = data.sections.flatMap(s => s.items).filter(i => i.response !== null).length

  return (
    <SafeAreaView style={styles.container} edges={['top']}>
      <ConnectivityBanner />
      <View style={styles.topBar}>
        <TouchableOpacity onPress={() => router.back()}>
          <Text style={styles.backText}>← Volver</Text>
        </TouchableOpacity>
        <Text style={styles.progress}>{answered}/{totalItems} respondidos</Text>
      </View>

      <ScrollView contentContainerStyle={styles.scroll}>
        {data.sections.map(section => (
          <View key={section.id} style={styles.section}>
            <Text style={styles.sectionTitle}>{section.name}</Text>
            {section.items.map(item => (
              <ChecklistItemCard
                key={item.id}
                item={item}
                onSubmit={(body) => mutation.mutate({ itemId: item.id, body })}
                isSubmitting={mutation.isPending}
              />
            ))}
          </View>
        ))}

        <TouchableOpacity
          style={styles.signatureBtn}
          onPress={() => router.push(`/(app)/signature/${data.inspectionId}`)}
        >
          <Text style={styles.signatureBtnText}>Capturar firma del cliente →</Text>
        </TouchableOpacity>
      </ScrollView>
    </SafeAreaView>
  )
}

function ChecklistItemCard({
  item, onSubmit, isSubmitting,
}: {
  item: ChecklistItemDto
  onSubmit: (body: object) => void
  isSubmitting: boolean
}) {
  const [textVal, setTextVal] = useState(item.response?.textValue ?? '')
  const [numVal, setNumVal] = useState(item.response?.numericValue?.toString() ?? '')
  const [notes, setNotes] = useState(item.response?.notes ?? '')

  const answered = item.response !== null
  const complies = item.response?.complies ?? true

  function submitYesNo(value: boolean) {
    onSubmit({ boolValue: value, complies: value, notes: notes || null })
  }

  function submitText() {
    if (!textVal.trim()) { Alert.alert('Requerido', 'Escribe una respuesta'); return }
    onSubmit({ textValue: textVal.trim(), complies: true, notes: notes || null })
  }

  function submitNumeric() {
    const num = parseFloat(numVal)
    if (isNaN(num)) { Alert.alert('Requerido', 'Ingresa un número válido'); return }
    onSubmit({ numericValue: num, complies: true, notes: notes || null })
  }

  return (
    <View style={[styles.card, answered && styles.cardAnswered]}>
      <View style={styles.cardHeader}>
        <Text style={styles.question}>{item.question}</Text>
        {item.isRequired && <Text style={styles.required}>*</Text>}
      </View>
      {item.helpText && <Text style={styles.helpText}>{item.helpText}</Text>}

      {answered && (
        <View style={[styles.badge, { backgroundColor: complies ? '#D1FAE5' : '#FEE2E2' }]}>
          <Text style={[styles.badgeText, { color: complies ? '#065F46' : '#991B1B' }]}>
            {complies ? '✓ Cumple' : '✗ No cumple'}
          </Text>
        </View>
      )}

      {item.itemType === 'YesNo' && (
        <View style={styles.yesNoRow}>
          <TouchableOpacity
            style={[styles.yesNoBtn, styles.yesBtn, item.response?.boolValue === true && styles.activeBtnYes]}
            onPress={() => submitYesNo(true)}
            disabled={isSubmitting}
          >
            <Text style={[styles.yesNoBtnText, item.response?.boolValue === true && styles.activeBtnText]}>Sí</Text>
          </TouchableOpacity>
          <TouchableOpacity
            style={[styles.yesNoBtn, styles.noBtn, item.response?.boolValue === false && styles.activeBtnNo]}
            onPress={() => submitYesNo(false)}
            disabled={isSubmitting}
          >
            <Text style={[styles.yesNoBtnText, item.response?.boolValue === false && styles.activeBtnText]}>No</Text>
          </TouchableOpacity>
        </View>
      )}

      {item.itemType === 'Text' && (
        <View>
          <TextInput
            style={styles.input}
            value={textVal}
            onChangeText={setTextVal}
            placeholder="Escribe tu respuesta..."
            multiline
          />
          <TouchableOpacity style={styles.submitBtn} onPress={submitText} disabled={isSubmitting}>
            <Text style={styles.submitBtnText}>Guardar</Text>
          </TouchableOpacity>
        </View>
      )}

      {item.itemType === 'Numeric' && (
        <View style={styles.numericRow}>
          <TextInput
            style={[styles.input, { flex: 1 }]}
            value={numVal}
            onChangeText={setNumVal}
            placeholder="0.00"
            keyboardType="decimal-pad"
          />
          <TouchableOpacity style={styles.submitBtn} onPress={submitNumeric} disabled={isSubmitting}>
            <Text style={styles.submitBtnText}>Guardar</Text>
          </TouchableOpacity>
        </View>
      )}

      {(item.itemType === 'Photo' || item.itemType === 'Signature') && (
        <Text style={styles.helpText}>
          {item.itemType === 'Photo' ? '📷 Usa el botón de evidencias en el detalle de la orden' : '✍️ Usa el botón de firma al final del checklist'}
        </Text>
      )}
    </View>
  )
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#F9FAFB' },
  center: { flex: 1, alignItems: 'center', justifyContent: 'center' },
  emptyText: { color: '#9CA3AF', fontSize: 15 },
  topBar: {
    flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center',
    padding: 16, backgroundColor: '#fff', borderBottomWidth: 1, borderBottomColor: '#E5E7EB',
  },
  backText: { fontSize: 15, color: '#1D4ED8', fontWeight: '500' },
  progress: { fontSize: 13, color: '#6B7280' },
  scroll: { padding: 16, gap: 16 },
  section: { gap: 12 },
  sectionTitle: {
    fontSize: 13, fontWeight: '700', color: '#374151',
    textTransform: 'uppercase', letterSpacing: 0.5,
    paddingHorizontal: 4,
  },
  card: {
    backgroundColor: '#fff', borderRadius: 12, padding: 16,
    elevation: 1, shadowColor: '#000', shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05, shadowRadius: 3, gap: 10,
  },
  cardAnswered: { borderLeftWidth: 3, borderLeftColor: '#10B981' },
  cardHeader: { flexDirection: 'row', gap: 4 },
  question: { flex: 1, fontSize: 14, fontWeight: '500', color: '#111827' },
  required: { color: '#EF4444', fontWeight: '700' },
  helpText: { fontSize: 12, color: '#9CA3AF' },
  badge: { alignSelf: 'flex-start', paddingHorizontal: 8, paddingVertical: 3, borderRadius: 99 },
  badgeText: { fontSize: 12, fontWeight: '600' },
  yesNoRow: { flexDirection: 'row', gap: 10 },
  yesNoBtn: {
    flex: 1, height: 40, borderRadius: 8, borderWidth: 1.5,
    alignItems: 'center', justifyContent: 'center',
  },
  yesBtn: { borderColor: '#10B981' },
  noBtn: { borderColor: '#EF4444' },
  activeBtnYes: { backgroundColor: '#10B981' },
  activeBtnNo: { backgroundColor: '#EF4444' },
  yesNoBtnText: { fontWeight: '600', fontSize: 14, color: '#374151' },
  activeBtnText: { color: '#fff' },
  input: {
    borderWidth: 1, borderColor: '#D1D5DB', borderRadius: 8,
    padding: 10, fontSize: 14, color: '#111827', backgroundColor: '#F9FAFB',
    minHeight: 44,
  },
  numericRow: { flexDirection: 'row', gap: 10, alignItems: 'center' },
  submitBtn: {
    backgroundColor: '#1D4ED8', borderRadius: 8,
    paddingHorizontal: 16, paddingVertical: 10, alignItems: 'center',
  },
  submitBtnText: { color: '#fff', fontWeight: '600', fontSize: 13 },
  signatureBtn: {
    backgroundColor: '#1D4ED8', borderRadius: 12, padding: 16,
    alignItems: 'center', marginTop: 8,
  },
  signatureBtnText: { color: '#fff', fontWeight: '700', fontSize: 15 },
})
