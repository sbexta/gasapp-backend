import { useRef, useState } from 'react'
import { View, Text, TouchableOpacity, StyleSheet, Alert, ActivityIndicator, ScrollView, TextInput } from 'react-native'
import { useLocalSearchParams, useRouter } from 'expo-router'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { SafeAreaView } from 'react-native-safe-area-context'
import SignatureCanvas from 'react-native-signature-canvas'
import { api } from '@/lib/api'

export default function SignatureScreen() {
  const { inspectionId } = useLocalSearchParams<{ inspectionId: string }>()
  const router = useRouter()
  const queryClient = useQueryClient()
  const sigRef = useRef<any>(null)
  const [signerName, setSignerName] = useState('')
  const [signerDocument, setSignerDocument] = useState('')
  const [signatureData, setSignatureData] = useState<string | null>(null)

  const mutation = useMutation({
    mutationFn: () =>
      api.post(`/inspections/${inspectionId}/signature`, {
        signerName: signerName.trim(),
        signatureData: signatureData!,
        signerDocument: signerDocument.trim() || null,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['inspection', inspectionId] })
      Alert.alert('Firma guardada', 'La firma del cliente fue registrada correctamente', [
        { text: 'OK', onPress: () => router.back() },
      ])
    },
    onError: (err: any) => {
      Alert.alert('Error', err.response?.data?.detail ?? 'No se pudo guardar la firma')
    },
  })

  function handleSave() {
    if (!signerName.trim()) { Alert.alert('Requerido', 'Ingresa el nombre del firmante'); return }
    if (!signatureData) { Alert.alert('Requerido', 'Dibuja la firma en el recuadro'); return }
    mutation.mutate()
  }

  function handleClear() {
    sigRef.current?.clearSignature()
    setSignatureData(null)
  }

  return (
    <SafeAreaView style={styles.container} edges={['top']}>
      <View style={styles.topBar}>
        <TouchableOpacity onPress={() => router.back()}>
          <Text style={styles.backText}>← Volver</Text>
        </TouchableOpacity>
        <Text style={styles.title}>Firma del cliente</Text>
      </View>

      <ScrollView contentContainerStyle={styles.scroll} keyboardShouldPersistTaps="handled">
        <Text style={styles.label}>Nombre del firmante *</Text>
        <TextInput
          style={styles.input}
          value={signerName}
          onChangeText={setSignerName}
          placeholder="Nombre completo"
          autoCapitalize="words"
        />

        <Text style={styles.label}>Documento (opcional)</Text>
        <TextInput
          style={styles.input}
          value={signerDocument}
          onChangeText={setSignerDocument}
          placeholder="Cédula o NIT"
          keyboardType="numeric"
        />

        <Text style={styles.label}>Firma *</Text>
        <View style={styles.signatureContainer}>
          <SignatureCanvas
            ref={sigRef}
            onOK={(sig) => setSignatureData(sig)}
            onEmpty={() => setSignatureData(null)}
            descriptionText=""
            clearText="Limpiar"
            confirmText="Guardar firma"
            webStyle={`
              .m-signature-pad { box-shadow: none; border: none; }
              .m-signature-pad--body { border: none; }
              .m-signature-pad--footer { display: none; }
              body { margin: 0; }
            `}
            style={{ flex: 1 }}
          />
          {signatureData && (
            <View style={styles.signedBadge}>
              <Text style={styles.signedText}>✓ Firma capturada</Text>
            </View>
          )}
        </View>

        <View style={styles.btnRow}>
          <TouchableOpacity style={styles.clearBtn} onPress={handleClear}>
            <Text style={styles.clearBtnText}>Limpiar</Text>
          </TouchableOpacity>
          <TouchableOpacity
            style={[styles.saveBtn, mutation.isPending && styles.saveBtnDisabled]}
            onPress={handleSave}
            disabled={mutation.isPending}
          >
            {mutation.isPending
              ? <ActivityIndicator color="#fff" />
              : <Text style={styles.saveBtnText}>Guardar firma</Text>
            }
          </TouchableOpacity>
        </View>
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
  scroll: { padding: 20, gap: 4 },
  label: { fontSize: 13, fontWeight: '500', color: '#374151', marginBottom: 6, marginTop: 12 },
  input: {
    height: 44, borderWidth: 1, borderColor: '#D1D5DB', borderRadius: 8,
    paddingHorizontal: 12, fontSize: 14, color: '#111827', backgroundColor: '#fff',
  },
  signatureContainer: {
    height: 250, borderWidth: 1.5, borderColor: '#D1D5DB', borderRadius: 12,
    overflow: 'hidden', backgroundColor: '#fff', marginTop: 8,
  },
  signedBadge: {
    position: 'absolute', top: 8, right: 8,
    backgroundColor: '#D1FAE5', paddingHorizontal: 10, paddingVertical: 4, borderRadius: 99,
  },
  signedText: { fontSize: 12, fontWeight: '600', color: '#065F46' },
  btnRow: { flexDirection: 'row', gap: 12, marginTop: 24 },
  clearBtn: {
    flex: 1, height: 48, borderRadius: 10, borderWidth: 1.5, borderColor: '#D1D5DB',
    alignItems: 'center', justifyContent: 'center',
  },
  clearBtnText: { fontSize: 15, fontWeight: '600', color: '#374151' },
  saveBtn: {
    flex: 2, height: 48, borderRadius: 10, backgroundColor: '#1D4ED8',
    alignItems: 'center', justifyContent: 'center',
  },
  saveBtnDisabled: { opacity: 0.6 },
  saveBtnText: { fontSize: 15, fontWeight: '700', color: '#fff' },
})
