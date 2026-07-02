import axios from 'axios'
import * as SecureStore from 'expo-secure-store'

// Cambia por la IP de tu PC (ver con ipconfig → Dirección IPv4)
// Usa http (no https) para evitar el certificado de desarrollo
const BASE_URL = 'http://192.168.0.11:5289/api/v1'

export const api = axios.create({
  baseURL: BASE_URL,
  timeout: 15000,
})

api.interceptors.request.use(async (config) => {
  const token = await SecureStore.getItemAsync('access_token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

api.interceptors.response.use(
  (res) => res,
  async (error) => {
    const original = error.config
    if (error.response?.status === 401 && !original._retry) {
      original._retry = true
      try {
        const refreshToken = await SecureStore.getItemAsync('refresh_token')
        const { data } = await axios.post(`${BASE_URL}/auth/refresh-token`, { refreshToken })
        await SecureStore.setItemAsync('access_token', data.accessToken)
        await SecureStore.setItemAsync('refresh_token', data.refreshToken)
        original.headers.Authorization = `Bearer ${data.accessToken}`
        return api(original)
      } catch {
        await SecureStore.deleteItemAsync('access_token')
        await SecureStore.deleteItemAsync('refresh_token')
      }
    }
    return Promise.reject(error)
  }
)
