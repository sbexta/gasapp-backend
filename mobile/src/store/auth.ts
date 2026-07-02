import { create } from 'zustand'
import * as SecureStore from 'expo-secure-store'

interface AuthUser {
  id: string
  email: string
  fullName: string
  role: string
}

interface AuthState {
  user: AuthUser | null
  accessToken: string | null
  isLoading: boolean
  setAuth: (user: AuthUser, accessToken: string, refreshToken: string) => Promise<void>
  clearAuth: () => Promise<void>
  loadAuth: () => Promise<void>
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  accessToken: null,
  isLoading: true,

  setAuth: async (user, accessToken, refreshToken) => {
    await SecureStore.setItemAsync('access_token', accessToken)
    await SecureStore.setItemAsync('refresh_token', refreshToken)
    await SecureStore.setItemAsync('auth_user', JSON.stringify(user))
    set({ user, accessToken })
  },

  clearAuth: async () => {
    await SecureStore.deleteItemAsync('access_token')
    await SecureStore.deleteItemAsync('refresh_token')
    await SecureStore.deleteItemAsync('auth_user')
    set({ user: null, accessToken: null })
  },

  loadAuth: async () => {
    try {
      const token = await SecureStore.getItemAsync('access_token')
      const userJson = await SecureStore.getItemAsync('auth_user')
      if (token && userJson) {
        set({ user: JSON.parse(userJson), accessToken: token })
      }
    } finally {
      set({ isLoading: false })
    }
  },
}))
