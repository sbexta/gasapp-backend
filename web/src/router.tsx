import { createRouter, createRoute, createRootRoute, redirect } from '@tanstack/react-router'
import { AppLayout } from '@/components/layout/AppLayout'
import { LoginPage } from '@/pages/LoginPage'
import { DashboardPage } from '@/pages/DashboardPage'
import { ClientsPage } from '@/pages/ClientsPage'
import { WorkOrdersPage } from '@/pages/WorkOrdersPage'

const rootRoute = createRootRoute()

const loginRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '/login',
  component: LoginPage,
})

const appRoute = createRoute({
  getParentRoute: () => rootRoute,
  id: 'app',
  component: AppLayout,
  beforeLoad: () => {
    const token = localStorage.getItem('access_token')
    if (!token) throw redirect({ to: '/login' })
  },
})

const indexRoute = createRoute({
  getParentRoute: () => appRoute,
  path: '/',
  beforeLoad: () => { throw redirect({ to: '/dashboard' }) },
})

const dashboardRoute = createRoute({
  getParentRoute: () => appRoute,
  path: '/dashboard',
  component: DashboardPage,
})

const clientsRoute = createRoute({
  getParentRoute: () => appRoute,
  path: '/clients',
  component: ClientsPage,
})

const workOrdersRoute = createRoute({
  getParentRoute: () => appRoute,
  path: '/work-orders',
  component: WorkOrdersPage,
})

const routeTree = rootRoute.addChildren([
  loginRoute,
  appRoute.addChildren([indexRoute, dashboardRoute, clientsRoute, workOrdersRoute]),
])

export const router = createRouter({ routeTree })

declare module '@tanstack/react-router' {
  interface Register {
    router: typeof router
  }
}
