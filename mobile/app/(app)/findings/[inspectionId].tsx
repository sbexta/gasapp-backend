import { useState } from 'react'
import {
  View, Text, ScrollView, TouchableOpacity, TextInput,
  StyleSheet, Alert, ActivityIndicator,
} from 'react-native'
import { useLocalSearchParams, useRouter } from 'expo-router'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { SafeAreaView } from 'react-native-safe-area-context'
import { api } from '@/lib/api'
import type { FindingDto } from '@/types/api'

const severityLabel: Record<string, string> = {
  Low: 'Baja', Medium: 'Media', High: 'Alta', Critical: 'Crítica',
}
const severityColor: Record<string, string> = {
  Low: '#10B981', Medium: '#F59E0B', High: '#EF4444', Critical: '#7C3AED',
}
const severities = ['Low', 'Medium', 'High', 'Critical'] as const

export default function FindingsScreen() {
  const { inspectionId } = useLocalSearchParams<{ inspectionId: string }>()
  const router = useRouter()
  const queryClient = useQueryClient()

  const [description, setDescription] = useState('')
  const [severity, setSeverity] = useState<typeof severities[number]>('Medium')
  const [requiresCorrection, setRequiresCorrection] = useState(false)
  const [correctiveAction, setCorrectiveAction] = useState('')

  const { data: findings, isLoading } = useQuery({
    queryKey: ['findings', inspectionId],
    queryFn: () =>
      api.get<{ findings: FindingDto[] }>(`/inspections/${inspectionId}`)
        .then(r => r.data.findings),
    enabled: !!inspectionId,
  })

  const mutation = useMutation({
    mutationFn: () =>
      api.post(`/inspections/${inspectionId}/findings`, {
        description: description.trim(),
        severity,
        requiresCorrection,
        correctiveAction: correctiveAction.trim() || null,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['findings', inspectionId] })
      setDescription('')
      setCorrectiveAction('')
      setRequiresCorrection(false)
      setSeverity('Medium')
      Alert.alert('Hallazgo registrado')
    },
    onError: () => Alert.alert('Error', 'No se pudo registrar el hallazgo'),
  })

  function handleSubmit() {
    if (!description.trim()) { Alert.alert('Requerido', 'Describe el hallazgo'); return }
    mutation.mutate()
  }

  return (
    <SafeAreaView style={styles.container} edges={['top']}>
      <View style={styles.topBar}>
        <TouchableOpacity onPress={() => router.back()}>
          <Text style={styles.backText}>← Volver</Text>
        </TouchableOpacity>
        <Text style={styles.title}>Hallazgos</Text>
      </View>

      <ScrollView contentContainerStyle={styles.scroll}>
        <View style={styles.form}>
          <Text style={styles.formTitle}>Nuevo hallazgo</Text>

          <Text style={styles.label}>Descripción *</Text>
          <TextInput
            style={[styles.input, styles.multiline]}
            value={description}
            onChangeText={setDescription}
            placeholder="Describe el hallazgo encontrado..."
            multiline
            numberOfLines={3}
          />

          <Text style={styles.label}>Severidad</Text>
          <View style={styles.severityRow}>
            {severities.map(s => (
              <TouchableOpacity
                key={s}
                style={[
                  styles.severityBtn,
                  { borderColor: severityColor[s] },
                  severity === s && { backgroundColor: severityColor[s] },
                ]}
                onPress={() => setSeverity(s)}
              >
                <Text style={[styles.severityText, severity === s && { color: '#fff' }]}>
                  {severityLabel[s]}
                </Text>
              </TouchableOpacity>
            ))}
          </View>

          <TouchableOpacity
            style={styles.checkRow}
            onPress={() => setRequiresCorrection(!requiresCorrection)}
          >
            <View style={[styles.checkbox, requiresCorrection && styles.checkboxActive]}>
              {requiresCorrection && <Text style={styles.checkmark}>✓</Text>}
            </View>
            <Text style={styles.checkLabel}>Requiere acción correctiva</Text>
          </TouchableOpacity>

          {requiresCorrection && (
            <>
              <Text style={styles.label}>Acción correctiva</Text>
              <TextInput
                style={[styles.input, styles.multiline]}
                value={correctiveAction}
                onChangeText={setCorrectiveAction}
                placeholder="Describe la acción a tomar..."
                multiline
                numberOfLines={2}
              />
            </>
          )}

          <TouchableOpacity
            style={[styles.submitBtn, mutation.isPending && styles.submitBtnDisabled]}
            onPress={handleSubmit}
            disabled={mutation.isPending}
          >
            {mutation.isPending
              ? <ActivityIndicator color="#fff" />
              : <Text style={styles.submitBtnText}>Registrar hallazgo</Text>
            }
          </TouchableOpacity>
        </View>

        <Text style={styles.sectionTitle}>
          Hallazgos registrados ({findings?.length ?? 0})
        </Text>

        {isLoading ? (
          <ActivityIndicator color="#1D4ED8" />
        ) : findings?.length === 0 ? (
          <Text style={styles.emptyText}>No hay hallazgos registrados</Text>
        ) : findings?.map(f => (
          <View key={f.id} style={[styles.findingCard, { borderLeftColor: severityColor[f.severity] }]}>
            <View style={styles.findingHeader}>
              <View style={[styles.badge, { backgroundColor: severityColor[f.severity] + '20' }]}>
                <Text style={[styles.badgeText, { color: severityColor[f.severity] }]}>
                  {severityLabel[f.severity]}
                </Text>
              </View>
              {f.isResolved && (
                <View style={[styles.badge, { backgroundColor: '#D1FAE5' }]}>
                  <Text style={[styles.badgeText, { color: '#065F46' }]}>Resuelto</Text>
                </View>
              )}
            </View>
            <Text style={styles.findingDesc}>{f.description}</Text>
            {f.correctiveAction && (
              <Text style={styles.findingAction}>→ {f.correctiveAction}</Text>
            )}
          </View>
        ))}
      </ScrollView>
    </SafeAreaView>
  )
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#F9FAFB' },
  topBar: {
    flexDirection: 'row', alignItems: 'center', gap: 16,
    padding: 16, backgroundColor: '#fff',
    borderBottomWidth: 1, borderBottomColor: '#E5E7EB',
  },
  backText: { fontSize: 15, color: '#1D4ED8', fontWeight: '500' },
  title: { fontSize: 17, fontWeight: '700', color: '#111827' },
  scroll: { padding: 16, gap: 16 },
  form: {
    backgroundColor: '#fff', borderRadius: 12, padding: 16,
    elevation: 2, shadowColor: '#000', shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05, shadowRadius: 4, gap: 8,
  },
  formTitle: { fontSize: 15, fontWeight: '700', color: '#111827', marginBottom: 4 },
  label: { fontSize: 13, fontWeight: '500', color: '#374151', marginTop: 8 },
  input: {
    borderWidth: 1, borderColor: '#D1D5DB', borderRadius: 8,
    paddingHorizontal: 12, paddingVertical: 10, fontSize: 14,
    color: '#111827', backgroundColor: '#F9FAFB',
  },
  multiline: { minHeight: 80, textAlignVertical: 'top' },
  severityRow: { flexDirection: 'row', flexWrap: 'wrap', gap: 8 },
  severityBtn: {
    paddingHorizontal: 12, paddingVertical: 6, borderRadius: 99, borderWidth: 1.5,
  },
  severityText: { fontSize: 12, fontWeight: '600', color: '#374151' },
  checkRow: { flexDirection: 'row', alignItems: 'center', gap: 10, marginTop: 8 },
  checkbox: {
    width: 22, height: 22, borderRadius: 4, borderWidth: 1.5,
    borderColor: '#D1D5DB', alignItems: 'center', justifyContent: 'center',
  },
  checkboxActive: { backgroundColor: '#1D4ED8', borderColor: '#1D4ED8' },
  checkmark: { color: '#fff', fontSize: 13, fontWeight: '700' },
  checkLabel: { fontSize: 14, color: '#374151' },
  submitBtn: {
    backgroundColor: '#1D4ED8', borderRadius: 10, padding: 14,
    alignItems: 'center', marginTop: 8,
  },
  submitBtnDisabled: { opacity: 0.6 },
  submitBtnText: { color: '#fff', fontWeight: '700', fontSize: 15 },
  sectionTitle: {
    fontSize: 13, fontWeight: '700', color: '#374151',
    textTransform: 'uppercase', letterSpacing: 0.5,
  },
  emptyText: { color: '#9CA3AF', fontSize: 14, textAlign: 'center', paddingVertical: 16 },
  findingCard: {
    backgroundColor: '#fff', borderRadius: 12, padding: 14,
    borderLeftWidth: 4, elevation: 1, gap: 6,
  },
  findingHeader: { flexDirection: 'row', gap: 8 },
  badge: { paddingHorizontal: 8, paddingVertical: 3, borderRadius: 99 },
  badgeText: { fontSize: 11, fontWeight: '600' },
  findingDesc: { fontSize: 14, color: '#111827' },
  findingAction: { fontSize: 13, color: '#6B7280', fontStyle: 'italic' },
})
