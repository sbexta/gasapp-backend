import axios from 'axios'
import * as SecureStore from 'expo-secure-store'

// En producción: define EXPO_PUBLIC_API_URL en .env (ej: https://gasapp.onrender.com)
// En desarrollo: usa la IP de tu PC (ipconfig → Dirección IPv4)
const BASE_URL = process.env.EXPO_PUBLIC_API_URL
  ? `${process.env.EXPO_PUBLIC_API_URL}/api/v1`
  : 'http://192.168.0.16:5289/api/v1'

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
