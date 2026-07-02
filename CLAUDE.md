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
    pages/                # LoginPage, DashboardPage, ClientsPage, WorkOrdersPage
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
    (app)/work-order/[id].tsx  # Detalle + botón iniciar
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
Proxy configurado: `/api` → `https://localhost:7051` (backend)

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

**Nota red local (mobile):** el backend escucha en `0.0.0.0:5289`. La IP del PC en `mobile/src/lib/api.ts` debe ser la IP del adaptador Ethernet (`192.168.0.11`). El celular debe estar en la misma red WiFi.

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
