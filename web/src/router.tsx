import { createRouter, createRoute, createRootRoute, redirect } from '@tanstack/react-router'
import { AppLayout } from '@/components/layout/AppLayout'
import { LoginPage } from '@/pages/LoginPage'
import { DashboardPage } from '@/pages/DashboardPage'
import { ClientsPage } from '@/pages/ClientsPage'
import { WorkOrdersPage } from '@/pages/WorkOrdersPage'
import { LocationsPage } from '@/pages/LocationsPage'
import { InspectionTypesPage } from '@/pages/InspectionTypesPage'
import { ContractsPage } from '@/pages/ContractsPage'
import { InspectionsPage } from '@/pages/InspectionsPage'
import { InspectionDetailPage } from '@/pages/InspectionDetailPage'
import { UsersPage } from '@/pages/UsersPage'
import { PermissionsPage } from '@/pages/PermissionsPage'

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

const contractsRoute = createRoute({
  getParentRoute: () => appRoute,
  path: '/contracts',
  component: ContractsPage,
})

const locationsRoute = createRoute({
  getParentRoute: () => appRoute,
  path: '/locations',
  component: LocationsPage,
})

const inspectionTypesRoute = createRoute({
  getParentRoute: () => appRoute,
  path: '/inspection-types',
  component: InspectionTypesPage,
})

const inspectionsRoute = createRoute({
  getParentRoute: () => appRoute,
  path: '/inspections',
  component: InspectionsPage,
})

const inspectionDetailRoute = createRoute({
  getParentRoute: () => appRoute,
  path: '/inspections/$id',
  component: InspectionDetailPage,
})

const usersRoute = createRoute({
  getParentRoute: () => appRoute,
  path: '/users',
  component: UsersPage,
})

const permissionsRoute = createRoute({
  getParentRoute: () => appRoute,
  path: '/permissions',
  component: PermissionsPage,
})

const routeTree = rootRoute.addChildren([
  loginRoute,
  appRoute.addChildren([
    indexRoute,
    dashboardRoute,
    clientsRoute,
    contractsRoute,
    locationsRoute,
    inspectionTypesRoute,
    workOrdersRoute,
    inspectionsRoute,
    inspectionDetailRoute,
    usersRoute,
    permissionsRoute,
  ]),
])

export const router = createRouter({ routeTree })

declare module '@tanstack/react-router' {
  interface Register {
    router: typeof router
  }
}
