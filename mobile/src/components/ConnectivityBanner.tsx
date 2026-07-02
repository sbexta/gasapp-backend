import { useEffect, useState } from 'react'
import { View, Text, StyleSheet } from 'react-native'
import NetInfo from '@react-native-community/netinfo'

export function ConnectivityBanner() {
  const [isConnected, setIsConnected] = useState<boolean | null>(true)

  useEffect(() => {
    const unsubscribe = NetInfo.addEventListener((state) => {
      setIsConnected(state.isConnected)
    })
    return unsubscribe
  }, [])

  if (isConnected !== false) return null

  return (
    <View style={styles.banner}>
      <Text style={styles.text}>Sin conexión — modo offline</Text>
    </View>
  )
}

const styles = StyleSheet.create({
  banner: {
    backgroundColor: '#F59E0B',
    paddingVertical: 6,
    alignItems: 'center',
  },
  text: {
    color: '#fff',
    fontSize: 12,
    fontWeight: '600',
  },
})
