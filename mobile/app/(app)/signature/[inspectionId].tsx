import { useRef, useState } from 'react'
import {
  View, Text, TouchableOpacity, StyleSheet, Alert,
  ActivityIndicator, ScrollView, TextInput, Modal, Image, Dimensions,
} from 'react-native'
import { useLocalSearchParams, useRouter } from 'expo-router'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { SafeAreaView } from 'react-native-safe-area-context'
import SignatureCanvas from 'react-native-signature-canvas'
import { api } from '@/lib/api'

const { width: SCREEN_W, height: SCREEN_H } = Dimensions.get('window')

export default function SignatureScreen() {
  const { inspectionId } = useLocalSearchParams<{ inspectionId: string }>()
  const router = useRouter()
  const queryClient = useQueryClient()
  const sigRef = useRef<any>(null)

  const [signerName, setSignerName] = useState('')
  const [signerDocument, setSignerDocument] = useState('')
  const [signatureData, setSignatureData] = useState<string | null>(null)
  const [showCanvas, setShowCanvas] = useState(false)
  const [hasDraw, setHasDraw] = useState(false)
  const pendingSave = useRef(false)

  const submitMutation = useMutation({
    mutationFn: (sigData: string) =>
      api.post(`/inspections/${inspectionId}/signature`, {
        signerName: signerName.trim(),
        signatureData: sigData,
        signerDocument: signerDocument.trim() || null,
      }).then(() =>
        api.post(`/inspections/${inspectionId}/submit`, { technicianNotes: null })
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['work-order'] })
      queryClient.invalidateQueries({ queryKey: ['agenda'] })
      Alert.alert(
        'Inspección finalizada',
        'La firma fue capturada y la inspección fue enviada para revisión.',
        [{ text: 'OK', onPress: () => router.replace('/(app)/agenda') }]
      )
    },
    onError: (err: any) => {
      Alert.alert('Error', err.response?.data?.message ?? 'No se pudo finalizar')
    },
  })

  function openCanvas() {
    if (!signerName.trim()) {
      Alert.alert('Requerido', 'Ingresa el nombre del firmante antes de firmar')
      return
    }
    setHasDraw(false)
    setShowCanvas(true)
  }

  function handleAccept() {
    if (!hasDraw) {
      Alert.alert('Requerido', 'Dibuja la firma antes de aceptar')
      return
    }
    pendingSave.current = true
    sigRef.current?.readSignature()
  }

  function handleClear() {
    sigRef.current?.clearSignature()
    setHasDraw(false)
  }

  function handleOK(sig: string) {
    if (!pendingSave.current) return
    pendingSave.current = false
    setSignatureData(sig)
    setShowCanvas(false)
  }

  function handleEmpty() {
    if (pendingSave.current) {
      pendingSave.current = false
      Alert.alert('Requerido', 'Dibuja la firma antes de aceptar')
    }
  }

  function handleGuardar() {
    if (!signerName.trim()) {
      Alert.alert('Requerido', 'Ingresa el nombre del firmante')
      return
    }
    if (!signatureData) {
      Alert.alert('Requerido', 'Captura la firma antes de guardar')
      return
    }
    submitMutation.mutate(signatureData)
  }

  // Dimensiones para el canvas rotado: ocupa toda la pantalla como si fuera landscape
  const canvasW = SCREEN_H   // ancho del canvas = alto de pantalla (paisaje)
  const canvasH = SCREEN_W   // alto del canvas  = ancho de pantalla (paisaje)

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

        {signatureData ? (
          <View style={styles.previewContainer}>
            <Image
              source={{ uri: signatureData }}
              style={styles.previewImage}
              resizeMode="contain"
            />
            <TouchableOpacity style={styles.resignBtn} onPress={openCanvas}>
              <Text style={styles.resignBtnText}>Volver a firmar</Text>
            </TouchableOpacity>
          </View>
        ) : (
          <TouchableOpacity style={styles.signBtn} onPress={openCanvas}>
            <Text style={styles.signBtnIcon}>✍️</Text>
            <Text style={styles.signBtnText}>Toca para firmar</Text>
            <Text style={styles.signBtnHint}>Se abrirá en modo horizontal</Text>
          </TouchableOpacity>
        )}

        <TouchableOpacity
          style={[styles.saveBtn, (!signatureData || submitMutation.isPending) && styles.btnDisabled]}
          onPress={handleGuardar}
          disabled={!signatureData || submitMutation.isPending}
        >
          {submitMutation.isPending
            ? <ActivityIndicator color="#fff" />
            : <Text style={styles.saveBtnText}>Guardar firma y finalizar inspección</Text>
          }
        </TouchableOpacity>
      </ScrollView>

      {/* Modal pantalla completa — canvas rotado 90° para simular landscape */}
      <Modal visible={showCanvas} animationType="fade" statusBarTranslucent>
        <View style={styles.modalBg}>
          {/* Contenedor rotado */}
          <View style={[styles.rotatedWrapper, { width: canvasW, height: canvasH }]}>

            {/* Barra superior (izquierda en real, arriba en visual) */}
            <View style={styles.canvasBar}>
              <TouchableOpacity onPress={() => setShowCanvas(false)} style={styles.canvasBarBtn}>
                <Text style={styles.canvasBarBtnText}>✕  Cancelar</Text>
              </TouchableOpacity>
              <Text style={styles.canvasBarTitle}>Firma aquí</Text>
              <View style={styles.canvasBarRight}>
                <TouchableOpacity onPress={handleClear} style={styles.canvasBarBtn}>
                  <Text style={styles.canvasBarBtnText}>🗑  Limpiar</Text>
                </TouchableOpacity>
                <TouchableOpacity onPress={handleAccept} style={styles.acceptBtn}>
                  <Text style={styles.acceptBtnText}>✓  Aceptar</Text>
                </TouchableOpacity>
              </View>
            </View>

            {/* Canvas */}
            <View style={{ flex: 1 }}>
              <SignatureCanvas
                ref={sigRef}
                onOK={handleOK}
                onEmpty={handleEmpty}
                onBegin={() => setHasDraw(true)}
                descriptionText=""
                webStyle={`
                  .m-signature-pad { box-shadow: none; border: none; width: 100%; height: 100%; }
                  .m-signature-pad--body { border: none; width: 100%; height: 100%; }
                  .m-signature-pad--footer { display: none; }
                  body { margin: 0; padding: 0; background: #fff; }
                `}
                style={{ flex: 1, backgroundColor: '#fff' }}
              />
            </View>

            {!hasDraw && (
              <View style={styles.hintOverlay}>
                <Text style={styles.hintText}>← Desliza para firmar →</Text>
              </View>
            )}
          </View>
        </View>
      </Modal>
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
  signBtn: {
    marginTop: 8, height: 120, borderWidth: 2, borderColor: '#BFDBFE',
    borderStyle: 'dashed', borderRadius: 12, backgroundColor: '#EFF6FF',
    alignItems: 'center', justifyContent: 'center', gap: 6,
  },
  signBtnIcon: { fontSize: 28 },
  signBtnText: { fontSize: 16, fontWeight: '700', color: '#1D4ED8' },
  signBtnHint: { fontSize: 12, color: '#93C5FD' },
  previewContainer: {
    marginTop: 8, borderWidth: 1.5, borderColor: '#D1FAE5',
    borderRadius: 12, overflow: 'hidden', backgroundColor: '#fff',
  },
  previewImage: { width: '100%', height: 160 },
  resignBtn: {
    padding: 10, alignItems: 'center',
    backgroundColor: '#F9FAFB', borderTopWidth: 1, borderTopColor: '#E5E7EB',
  },
  resignBtnText: { fontSize: 13, color: '#6B7280', fontWeight: '500' },
  saveBtn: {
    marginTop: 28, height: 52, borderRadius: 12, backgroundColor: '#059669',
    alignItems: 'center', justifyContent: 'center',
  },
  btnDisabled: { opacity: 0.4 },
  saveBtnText: { fontSize: 16, fontWeight: '700', color: '#fff' },

  // Modal
  modalBg: {
    flex: 1, backgroundColor: '#000',
    alignItems: 'center', justifyContent: 'center',
  },
  rotatedWrapper: {
    transform: [{ rotate: '90deg' }],
    overflow: 'hidden',
    flexDirection: 'column',
  },
  canvasBar: {
    flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between',
    backgroundColor: '#1E293B', paddingHorizontal: 16, paddingVertical: 10,
  },
  canvasBarTitle: { fontSize: 15, fontWeight: '700', color: '#fff' },
  canvasBarRight: { flexDirection: 'row', gap: 10, alignItems: 'center' },
  canvasBarBtn: {
    paddingHorizontal: 12, paddingVertical: 6,
    borderRadius: 8, backgroundColor: 'rgba(255,255,255,0.12)',
  },
  canvasBarBtnText: { color: '#CBD5E1', fontWeight: '600', fontSize: 13 },
  acceptBtn: {
    paddingHorizontal: 16, paddingVertical: 6,
    borderRadius: 8, backgroundColor: '#059669',
  },
  acceptBtnText: { color: '#fff', fontWeight: '700', fontSize: 13 },
  hintOverlay: {
    position: 'absolute', bottom: 16, left: 0, right: 0, alignItems: 'center',
  },
  hintText: { color: '#9CA3AF', fontSize: 13 },
})
