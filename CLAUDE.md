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

### Mobile (`mobile/`) — Sprint 4 pendiente
| | |
|---|---|
| Framework | Expo SDK (React Native) |
| Routing | Expo Router |
| Offline | WatermelonDB |
| Auth storage | expo-secure-store |
| **Requisito** | Node ≥ 20.19.4 (actualmente 20.15.1) |

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

### Sprint 4 — PENDIENTE
**Scope:** App móvil Expo (React Native)

**Prerequisitos antes de empezar:**
1. Actualizar Node.js a ≥ 20.19.4 (actualmente 20.15.1) → https://nodejs.org
2. Para emulador Android: instalar Android Studio
3. Para probar sin emulador: instalar **Expo Go** en el celular (Android o iOS)

**Funcionalidades planificadas:**
- Login con token en `expo-secure-store`
- Agenda del técnico (inspecciones del día)
- Detalle de orden de trabajo
- Descarga offline con WatermelonDB
- Cambio de estado `Assigned → InProgress`
- Indicador de conectividad
- Sincronización básica al reconectar

**Nota:** el mismo código funciona para Android e iOS. En Windows solo se puede probar con Expo Go en teléfono físico (emulador iOS requiere Mac/Xcode).

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

# 3. Correr la API (puerto https: 7051, http: 5289)
dotnet run --project src/GasApp.API
# Swagger: https://localhost:7051/swagger
# Credenciales admin: admin@gasapp.com / Admin1234!

# 4. Correr el web admin
cd web && npm run dev
# Abre: http://localhost:3000
```

```bash
# Correr tests
dotnet test
```

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
