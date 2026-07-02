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
  const [signatureSaved, setSignatureSaved] = useState(false)
  const [hasDraw, setHasDraw] = useState(false)
  // guardamos si el usuario presionó "Guardar" para saber si ejecutar la mutación al llegar onOK
  const pendingSave = useRef(false)

  const signatureMutation = useMutation({
    mutationFn: (sigData: string) =>
      api.post(`/inspections/${inspectionId}/signature`, {
        signerName: signerName.trim(),
        signatureData: sigData,
        signerDocument: signerDocument.trim() || null,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['inspection', inspectionId] })
      setSignatureSaved(true)
    },
    onError: (err: any) => {
      Alert.alert('Error', err.response?.data?.detail ?? 'No se pudo guardar la firma')
    },
  })

  const submitMutation = useMutation({
    mutationFn: () =>
      api.post(`/inspections/${inspectionId}/submit`, { technicianNotes: null }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['work-order'] })
      queryClient.invalidateQueries({ queryKey: ['agenda'] })
      Alert.alert(
        'Inspección finalizada',
        'La inspección fue enviada para revisión correctamente.',
        [{ text: 'OK', onPress: () => router.replace('/(app)/agenda') }]
      )
    },
    onError: (err: any) => {
      Alert.alert('Error', err.response?.data?.detail ?? 'No se pudo finalizar la inspección')
    },
  })

  function handleSave() {
    if (!signerName.trim()) {
      Alert.alert('Requerido', 'Ingresa el nombre del firmante')
      return
    }
    if (!hasDraw) {
      Alert.alert('Requerido', 'Dibuja la firma en el recuadro')
      return
    }
    pendingSave.current = true
    // readSignature dispara onOK con el base64 si hay trazos, o onEmpty si está vacío
    sigRef.current?.readSignature()
  }

  function handleClear() {
    sigRef.current?.clearSignature()
    setHasDraw(false)
  }

  // Se dispara cuando readSignature() encuentra trazos
  function handleOK(sig: string) {
    if (pendingSave.current) {
      pendingSave.current = false
      signatureMutation.mutate(sig)
    }
  }

  // Se dispara cuando readSignature() encuentra el canvas vacío
  function handleEmpty() {
    if (pendingSave.current) {
      pendingSave.current = false
      Alert.alert('Requerido', 'Dibuja la firma en el recuadro')
    }
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
        {!signatureSaved ? (
          <>
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
                onOK={handleOK}
                onEmpty={handleEmpty}
                onBegin={() => setHasDraw(true)}
                descriptionText=""
                clearText="Limpiar"
                confirmText="Confirmar"
                webStyle={`
                  .m-signature-pad { box-shadow: none; border: none; }
                  .m-signature-pad--body { border: none; }
                  .m-signature-pad--footer { display: none; }
                  body { margin: 0; }
                `}
                style={{ flex: 1 }}
              />
              {hasDraw && (
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
                style={[styles.saveBtn, signatureMutation.isPending && styles.btnDisabled]}
                onPress={handleSave}
                disabled={signatureMutation.isPending}
              >
                {signatureMutation.isPending
                  ? <ActivityIndicator color="#fff" />
                  : <Text style={styles.saveBtnText}>Guardar firma</Text>
                }
              </TouchableOpacity>
            </View>
          </>
        ) : (
          <View style={styles.successContainer}>
            <View style={styles.successIcon}>
              <Text style={styles.successIconText}>✓</Text>
            </View>
            <Text style={styles.successTitle}>Firma registrada</Text>
            <Text style={styles.successSubtitle}>
              La firma de {signerName} fue guardada correctamente.
            </Text>
            <TouchableOpacity
              style={[styles.finishBtn, submitMutation.isPending && styles.btnDisabled]}
              onPress={() => submitMutation.mutate()}
              disabled={submitMutation.isPending}
            >
              {submitMutation.isPending
                ? <ActivityIndicator color="#fff" />
                : <Text style={styles.finishBtnText}>Finalizar inspección →</Text>
              }
            </TouchableOpacity>
            <TouchableOpacity style={styles.backLink} onPress={() => router.back()}>
              <Text style={styles.backLinkText}>Volver al detalle sin finalizar</Text>
            </TouchableOpacity>
          </View>
        )}
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
  btnDisabled: { opacity: 0.6 },
  saveBtnText: { fontSize: 15, fontWeight: '700', color: '#fff' },
  successContainer: { alignItems: 'center', paddingTop: 40, paddingHorizontal: 16 },
  successIcon: {
    width: 72, height: 72, borderRadius: 36, backgroundColor: '#D1FAE5',
    alignItems: 'center', justifyContent: 'center', marginBottom: 16,
  },
  successIconText: { fontSize: 32, color: '#065F46' },
  successTitle: { fontSize: 20, fontWeight: '700', color: '#111827', marginBottom: 8 },
  successSubtitle: { fontSize: 14, color: '#6B7280', textAlign: 'center', marginBottom: 32 },
  finishBtn: {
    width: '100%', height: 52, borderRadius: 12, backgroundColor: '#059669',
    alignItems: 'center', justifyContent: 'center', marginBottom: 12,
  },
  finishBtnText: { fontSize: 16, fontWeight: '700', color: '#fff' },
  backLink: { paddingVertical: 8 },
  backLinkText: { fontSize: 13, color: '#9CA3AF' },
})
