# GasApp — Contexto del Proyecto

## Descripción
Sistema de inspección de gas en Colombia. API REST + Web Admin + App móvil.

**Repo GitHub:** https://github.com/sbexta/gasapp-backend.git  
**Branch principal:** `main`

---

## Stack técnico

### Backend
| Capa | Tecnología |
|------|-----------|
| Framework | ASP.NET Core 8 Web API |
| Arquitectura | Clean Architecture + CQRS (MediatR 12.4.1) |
| Validación | FluentValidation 11.10.0 (pipeline behavior) |
| ORM | Entity Framework Core 8 + Npgsql |
| Auth | JWT Bearer (HS256) — access 15 min + refresh 30 días |
| Hashing | BCrypt work factor 12 |
| BD | PostgreSQL 16 (Docker) |
| Cache | Redis 7 (Docker) |
| Tests | xUnit + FluentAssertions + Moq |

### Web Admin (`web/`)
| | |
|---|---|
| Framework | React 18 + Vite 5 + TypeScript |
| Routing | TanStack Router |
| Data fetching | TanStack Query |
| Estilos | Tailwind CSS v4 |
| Componentes | Radix UI + lucide-react |
| Forms | React Hook Form + Zod |
| Estado global | Zustand (auth) |

### Mobile (`mobile/`)
| | |
|---|---|
| Framework | Expo SDK 54 (React Native) |
| Routing | Expo Router 4 |
| Auth storage | expo-secure-store |
| Data fetching | TanStack Query + Axios |
| Estado global | Zustand |
| Conectividad | @react-native-community/netinfo |
| Pruebas | Expo Go en dispositivo físico |

---

## Estructura de proyectos

```
src/
  GasApp.Domain/          # Entidades, Value Objects, Enums, Excepciones, Repos (interfaces)
  GasApp.Application/     # Commands, Queries, Handlers, Validators, Behaviors
  GasApp.Infrastructure/  # EF Core, Repos, JWT, BCrypt, Migrations
  GasApp.API/             # Controllers, Middleware, Program.cs

web/                      # Web admin React
  src/
    components/           # UI (Button, Card, Input...) + layout (Sidebar, AppLayout)
    pages/                # LoginPage, DashboardPage, ClientsPage, ContractsPage,
                          # LocationsPage, InspectionTypesPage, WorkOrdersPage
    lib/                  # api.ts (axios + interceptors), utils.ts
    store/                # auth.ts (Zustand)
    types/                # api.ts (DTOs tipados)
    router.tsx            # TanStack Router (rutas protegidas)

mobile/                   # App móvil Expo
  app/
    _layout.tsx           # Root layout + AuthGuard
    (auth)/login.tsx      # Pantalla de login
    (app)/agenda.tsx      # Agenda del día del técnico
    (app)/work-orders.tsx # Listado de órdenes
    (app)/work-order/[id].tsx   # Detalle + botón iniciar
    (app)/checklist/[workOrderId].tsx  # Checklist de inspección
    (app)/findings/[inspectionId].tsx  # Hallazgos
    (app)/signature/[inspectionId].tsx # Firma del cliente
  src/
    lib/api.ts            # Axios + interceptores JWT
    store/auth.ts         # Zustand con expo-secure-store
    types/api.ts          # DTOs tipados
    components/ConnectivityBanner.tsx  # Indicador offline

tests/
  GasApp.Domain.Tests/
  GasApp.Application.Tests/
  GasApp.API.IntegrationTests/
```

**Dependencias backend:** API → Application + Infrastructure → Domain (Domain sin referencias externas)

---

## Sprints

### Sprint 1 — COMPLETADO ✅
**Commit:** `dd02dce`, `c8c59d5`

- `AuditableEntity`, `User`, `UserSession` — dominio base
- Value Objects: `Email`, `GeoPoint`
- Enums: `UserRole`, `InspectionStatus`, `WorkOrderStatus`
- `InspectionStateMachine` — FSM con transiciones y permisos por rol
- Excepciones: `DomainException`, `NotFoundException`, `InvalidStateTransitionException`
- Auth: Login, RefreshToken, Logout (JWT + BCrypt)
- Users: CRUD completo con autorización por rol
- `ExceptionHandlingMiddleware`, `CurrentUserMiddleware`
- Swagger con Bearer security
- `docker-compose.yml` — PostgreSQL 16 + Redis 7
- Migración: `20260623013726_InitialSchema`
- Tests: 12 tests `InspectionStateMachineTests` ✅

---

### Sprint 2 — COMPLETADO ✅
**Commit:** `c21ee6b`

**Entidades de dominio (11 nuevas):**
- `Client`, `Contract`, `Location`, `Installation` — jerarquía cliente→contrato→sede→instalación
- `InspectionType`, `WorkOrder`, `Inspection` — ciclo operativo con FSM integrado
- `ChecklistTemplate`, `ChecklistSection`, `ChecklistItem`

**Nuevos enums:** `ClientType`, `ContractStatus`, `InstallationType`, `InstallationStatus`, `ChecklistItemType`

**Application:** 14 Commands + 4 Queries con handlers y validadores

**Infrastructure:** 7 repositorios, 8 configuraciones EF Core, migración `20260623202741_Sprint2Schema` (9 tablas)

**API — 5 controllers nuevos:**
- `ClientsController` — GET/POST/PUT, autorización por rol
- `ContractsController` — POST
- `LocationsController` — POST
- `InstallationsController` — POST
- `WorkOrdersController` — GET/POST + POST /{id}/assign

**Seeder:** `DbSeeder` — crea usuario admin en Development
- Email: `admin@gasapp.com` / Password: `Admin1234!`

---

### Sprint 3 — COMPLETADO ✅
**Commit:** `dc11492`

**Web admin en `web/`:**
- Setup Vite 5 + React 18 + TypeScript + Tailwind CSS v4
- TanStack Router con rutas protegidas (redirige a /login si no hay token)
- Refresh automático de JWT via interceptor de Axios
- Zustand para estado de autenticación (persistido en localStorage)
- **LoginPage** — form con validación Zod, manejo de errores
- **DashboardPage** — stats (clientes, órdenes) + tabla de órdenes recientes
- **ClientsPage** — listado con búsqueda + modal para crear cliente
- **WorkOrdersPage** — listado con filtro por estado
- **Sidebar** — navegación con filtro por rol, info de usuario, logout
- Componentes UI: Button, Input, Label, Card, Badge

**Cómo correr el web:**
```bash
cd web
npm run dev
# Abre http://localhost:3000
```
Proxy configurado: `/api` → `http://localhost:5289` (backend)

---

### Sprint 4 — COMPLETADO ✅
**Commits:** `95daf49`, `eacb35f`

**App móvil en `mobile/`:**
- Expo SDK 54 + Expo Router 4 + TypeScript
- Auth con `expo-secure-store` — token guardado de forma segura en el dispositivo
- Interceptor Axios para refresh automático de JWT
- Zustand para estado de autenticación
- **LoginScreen** — form con validación, manejo de errores
- **AgendaScreen** — órdenes del día del técnico con pull-to-refresh
- **WorkOrdersScreen** — listado de todas las órdenes asignadas
- **WorkOrderDetailScreen** — detalle completo (cliente, sede, fecha) + botón **Iniciar inspección**
- **ConnectivityBanner** — banner naranja cuando no hay conexión (NetInfo)
- Tab navigation: Agenda | Órdenes
- Probado en dispositivo físico con Expo Go ✅

**Nuevos endpoints backend:**
- `GET /api/v1/work-orders/{id}` — detalle con cliente y sede
- `POST /api/v1/work-orders/{id}/start` — cambia estado `Assigned → InProgress`
- `GET /api/v1/work-orders/agenda` — usa el ID del token automáticamente para técnicos

---

### Sprint 5 — COMPLETADO ✅
**Commit:** `6e8efb2`

**Ejecución del checklist en mobile:**
- **ChecklistScreen** (`/(app)/checklist/[workOrderId]`) — renderiza secciones e ítems con respuestas tipo YesNo (botones), Text (TextInput) y Numeric (teclado decimal); progreso `{respondidos}/{total}`; navega a firma al terminar
- **FindingsScreen** (`/(app)/findings/[inspectionId]`) — formulario para registrar hallazgos (descripción, severidad Low/Medium/High/Critical, acción correctiva); lista hallazgos existentes
- **SignatureScreen** (`/(app)/signature/[inspectionId]`) — canvas táctil con `react-native-signature-canvas`; campos nombre y documento del firmante; guarda base64 PNG
- **WorkOrderDetailScreen** actualizado — botones "Checklist" y "Hallazgos" visibles cuando estado = `InProgress`

**Nuevos endpoints backend:**
- `GET /api/v1/inspections/{id}` — detalle con hallazgos
- `GET /api/v1/inspections/by-work-order/{workOrderId}/checklist` — checklist con respuestas actuales
- `POST /api/v1/inspections/{id}/responses` — upsert de respuesta a ítem (crea o actualiza)
- `POST /api/v1/inspections/{id}/findings` — registrar hallazgo
- `POST /api/v1/inspections/{id}/signature` — capturar firma (base64 PNG)
- `POST /api/v1/inspections/{id}/evidences` — subir evidencia (multipart)

**Infrastructure:**
- `LocalFileStorageService` — guarda evidencias en `uploads/{inspectionId}/{guid}_{fileName}`
- Repositorios: `IChecklistResponseRepository`, `IEvidenceRepository`, `IFindingRepository`, `IInspectionSignatureRepository`

---

### Sprint 6 — COMPLETADO ✅
**Commit:** `7f0dce3`

**Web admin — módulos de gestión completos:**
- **ContractsPage** (`/contracts`) — tabla de contratos con filtro por cliente + modal "Nuevo contrato" (cliente, N° contrato, fechas inicio/fin con validación, notas)
- **LocationsPage** (`/locations`) — tabla de sedes con modal "Nueva sede" (selección cascada cliente → contrato → nombre/dirección/municipio/departamento)
- **InspectionTypesPage** (`/inspection-types`) — tabla de tipos + modal "Nuevo tipo" (nombre, descripción, requiere certificado)
- **WorkOrdersPage** actualizado — modal "Nueva orden" (sede, tipo inspección, fecha) + acción "Asignar técnico" por fila con selector de técnicos activos
- **Sidebar** — nuevos links: Contratos, Sedes, Tipos de inspección

**Nuevos endpoints backend:**
- `GET /api/v1/contracts` — lista contratos con nombre de cliente, filtrable por `?clientId`
- `GET /api/v1/locations` — lista sedes activas con nombre del cliente
- `GET /api/v1/inspection-types` — lista tipos de inspección activos
- `POST /api/v1/inspection-types` — crear tipo de inspección

**Infrastructure:**
- `IInspectionTypeRepository` + `InspectionTypeRepository` — nuevo repositorio
- `GetContractsQuery`, `GetLocationsQuery`, `GetInspectionTypesQuery` — nuevas queries
- `CreateInspectionTypeCommand` — nuevo command

**Mobile — fix:**
- Corregido warning "Each child in a list should have a unique key prop" en `WorkOrdersScreen`: el endpoint `/work-orders` devuelve `id` (no `workOrderId`); se creó `WorkOrderSummaryDto` y se actualizó `keyExtractor`

**Flujo completo sin Swagger:**
Clientes → Contratos → Sedes → Tipos de inspección → Órdenes de trabajo → Asignar técnico

---

### Sprint 7 — COMPLETADO ✅
**Commit:** (este commit)

**Flujo de finalización de inspecciones — backend:**
- **`SubmitInspectionCommand`** — técnico envía inspección: `InProgress → TechnicalReview`
- **`ApproveInspectionCommand`** — supervisor aprueba: `TechnicalReview → GeneratingDocs → Completed` (dos transiciones en un handler)
- **`GetInspectionsQuery`** — paginada, filtrable por status, retorna `PagedResult<InspectionListDto>` con `orderNumber` y `scheduledDate`
- **`GetInspectionDetailQuery`** — extendido con `orderNumber` y `scheduledDate` del WorkOrder
- **`StartWorkOrderHandler`** — fix crítico: ahora crea el `Inspection` automáticamente al iniciar la OT (Pending → PreCheck → InProgress)
- **`GetWorkOrderByIdQuery`** — incluye `inspectionId` en el DTO para que mobile pueda navegar directamente
- **`JsonStringEnumConverter`** en `Program.cs` — fix: enums se serializan/deserializan como string (no int)

**Permisos por rol:**
- **`AppPermission` enum** (15 permisos), **`RolePermission` entity**, migración `Sprint7_RolePermissions`
- **`GetRolePermissionsQuery`** → matriz completa de 5 roles × 15 permisos
- **`UpdateRolePermissionCommand`** → upsert (crear si no existe, actualizar si existe)
- **`RolePermissionsController`** — `GET/PUT /api/v1/role-permissions` (Admin only)

**Web admin — nuevas páginas:**
- **InspectionsPage** (`/inspections`) — tabla con filtro por estado, navega al detalle
- **InspectionDetailPage** (`/inspections/$id`) — hallazgos con colores por severidad, respuestas de checklist, estado firma, modal aprobar con notas de supervisor
- **UsersPage** (`/users`) — CRUD completo con filtros rol/estado, modal crear con validación Zod (email, password ≥8 chars + mayúscula + número)
- **PermissionsPage** (`/permissions`) — matriz visual editable por grupo, toggle inmediato vía API
- **WorkOrdersPage** — botón "Iniciar inspección" para OTs en estado `Assigned`

**Mobile — restructuración de tabs y fixes:**
- **Estructura nueva**: `(app)/_layout.tsx` = Stack; `(app)/(tabs)/_layout.tsx` = Tabs solo con Agenda y Órdenes
- **Signature fix**: `handleSave()` llama `readSignature()`, `onOK` ejecuta mutación, `onEmpty` muestra alerta — firma funciona correctamente
- **Findings fix**: `JsonStringEnumConverter` en backend (enum `FindingSeverity` ya no falla)
- **Navigation fix**: `work-order/[id].tsx` navega a `/findings/{inspectionId}` (no `workOrderId`)

**Flujo completo verificado vía API:**
Crear OT → Asignar técnico → Iniciar → Registrar hallazgo → Capturar firma → Submit → Aprobar admin → Estado `Completed`

---

## Convenciones del proyecto

- Tablas en `snake_case` (EF Core convención)
- Enums almacenados como string en BD
- Soft delete vía `DeletedAt` nullable + query filter automático
- Commands/Queries en carpetas por módulo: `Application/{Modulo}/Commands/{NombreCommand}/`
- Cada Command/Query tiene su Handler y Validator en la misma carpeta
- Errores del dominio siempre via excepciones tipadas (nunca Result<T> en dominio)
- JWT: token crudo nunca persiste — solo hash SHA-256
- NIT colombiano formato: `123456789-0` (9 dígitos + guión + 1 dígito verificador)

---

## Cómo correr el proyecto

```bash
# 1. Levantar BD y Redis
docker compose up -d

# 2. Aplicar migraciones
dotnet ef database update --project src/GasApp.Infrastructure --startup-project src/GasApp.API

# 3. Correr la API (accesible desde la red local)
dotnet run --project src/GasApp.API --launch-profile http
# Swagger: http://localhost:5289/swagger
# Credenciales admin: admin@gasapp.com / Admin1234!

# 4. Correr el web admin
cd web && npm run dev
# Abre: http://localhost:3000

# 5. Correr la app móvil
cd mobile && npx expo start --clear
# Escanear QR con Expo Go (celular en la misma red WiFi que el PC)
```

```bash
# Correr tests
dotnet test
```

**Nota red local (mobile):** el backend escucha en `0.0.0.0:5289`. La IP del PC en `mobile/src/lib/api.ts` debe ser la IP WiFi del PC (ver con `ipconfig` → adaptador Wi-Fi → Dirección IPv4). El celular debe estar en la misma red WiFi.

---

## Variables de configuración (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=gasapp;Username=gasapp;Password=gasapp123"
  },
  "Jwt": {
    "Secret": "CAMBIAR_POR_SECRET_SEGURO_EN_PRODUCCION_MIN_32_CHARS",
    "Issuer": "GasApp.API",
    "Audience": "GasApp.Clients",
    "AccessTokenExpiryMinutes": "15"
  }
}
```
