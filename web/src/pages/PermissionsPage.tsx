import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'

interface PermissionMatrix {
  roles: string[]
  permissions: string[]
  matrix: Record<string, Record<string, boolean>>
}

const permissionLabel: Record<string, string> = {
  ManageUsers: 'Gestionar usuarios',
  ViewUsers: 'Ver usuarios',
  ManageClients: 'Gestionar clientes',
  ViewClients: 'Ver clientes',
  ManageContracts: 'Gestionar contratos',
  ManageLocations: 'Gestionar sedes',
  ManageInspectionTypes: 'Gestionar tipos inspección',
  ManageWorkOrders: 'Gestionar órdenes',
  AssignTechnicians: 'Asignar técnicos',
  StartInspections: 'Iniciar inspecciones',
  PerformChecklist: 'Ejecutar checklist',
  RegisterFindings: 'Registrar hallazgos',
  CaptureSignature: 'Capturar firma',
  ViewInspections: 'Ver inspecciones',
  ApproveInspections: 'Aprobar inspecciones',
}

const roleLabel: Record<string, string> = {
  Admin: 'Admin',
  Supervisor: 'Supervisor',
  Technician: 'Técnico',
  ClientUser: 'Cliente',
  Auditor: 'Auditor',
}

const permissionGroups: Record<string, string[]> = {
  'Usuarios': ['ManageUsers', 'ViewUsers'],
  'Clientes y contratos': ['ManageClients', 'ViewClients', 'ManageContracts', 'ManageLocations'],
  'Operaciones': ['ManageInspectionTypes', 'ManageWorkOrders', 'AssignTechnicians', 'StartInspections'],
  'Ejecución': ['PerformChecklist', 'RegisterFindings', 'CaptureSignature'],
  'Revisión': ['ViewInspections', 'ApproveInspections'],
}

export function PermissionsPage() {
  const qc = useQueryClient()

  const { data, isLoading } = useQuery({
    queryKey: ['role-permissions'],
    queryFn: () => api.get<PermissionMatrix>('/role-permissions').then((r) => r.data),
  })

  const toggleMutation = useMutation({
    mutationFn: ({ role, permission, isGranted }: { role: string; permission: string; isGranted: boolean }) =>
      api.put('/role-permissions', { role, permission, isGranted }),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['role-permissions'] }),
  })

  if (isLoading || !data) {
    return <div className="p-8 text-gray-400">Cargando permisos...</div>
  }

  function toggle(role: string, permission: string) {
    const current = data!.matrix[role]?.[permission] ?? false
    toggleMutation.mutate({ role, permission, isGranted: !current })
  }

  return (
    <div className="p-8">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Permisos por rol</h1>
        <p className="text-sm text-gray-500">Define qué acciones puede realizar cada rol en el sistema</p>
      </div>

      <div className="overflow-x-auto rounded-lg border border-gray-200 bg-white">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b border-gray-200 bg-gray-50">
              <th className="px-4 py-3 text-left font-medium text-gray-600 min-w-[200px]">Permiso</th>
              {data.roles.map((role) => (
                <th key={role} className="px-4 py-3 text-center font-medium text-gray-600 min-w-[110px]">
                  {roleLabel[role] ?? role}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {Object.entries(permissionGroups).map(([group, permissions]) => (
              <>
                <tr key={`group-${group}`} className="bg-gray-50 border-t border-gray-200">
                  <td
                    colSpan={data.roles.length + 1}
                    className="px-4 py-2 text-xs font-semibold text-gray-500 uppercase tracking-wide"
                  >
                    {group}
                  </td>
                </tr>
                {permissions.filter((p) => data.permissions.includes(p)).map((permission) => (
                  <tr key={permission} className="border-t border-gray-100 hover:bg-gray-50">
                    <td className="px-4 py-3 text-gray-700">
                      {permissionLabel[permission] ?? permission}
                    </td>
                    {data.roles.map((role) => {
                      const granted = data.matrix[role]?.[permission] ?? false
                      const isPending = toggleMutation.isPending &&
                        toggleMutation.variables?.role === role &&
                        toggleMutation.variables?.permission === permission
                      return (
                        <td key={role} className="px-4 py-3 text-center">
                          <button
                            onClick={() => toggle(role, permission)}
                            disabled={isPending}
                            className={`inline-flex h-6 w-6 items-center justify-center rounded-full text-sm font-bold transition-colors disabled:opacity-50 ${
                              granted
                                ? 'bg-green-100 text-green-700 hover:bg-green-200'
                                : 'bg-gray-100 text-gray-400 hover:bg-gray-200'
                            }`}
                            title={granted ? 'Quitar permiso' : 'Dar permiso'}
                          >
                            {granted ? '✓' : '×'}
                          </button>
                        </td>
                      )
                    })}
                  </tr>
                ))}
              </>
            ))}
          </tbody>
        </table>
      </div>

      <p className="mt-4 text-xs text-gray-400">
        Los cambios se guardan inmediatamente. Los permisos aplican en el siguiente inicio de sesión del usuario.
      </p>
    </div>
  )
}
