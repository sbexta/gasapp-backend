# GasApp Backend — Contexto del Proyecto

## Descripción
Sistema de inspección de gas en Colombia. API REST en ASP.NET Core 8 con Clean Architecture + CQRS.

**Repo GitHub:** https://github.com/sbexta/gasapp-backend.git  
**Branch principal:** `main`

---

## Stack técnico

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

---

## Estructura de proyectos

```
src/
  GasApp.Domain/          # Entidades, Value Objects, Enums, Excepciones, Repos (interfaces)
  GasApp.Application/     # Commands, Queries, Handlers, Validators, Behaviors
  GasApp.Infrastructure/  # EF Core, Repos, JWT, BCrypt, Migrations
  GasApp.API/             # Controllers, Middleware, Program.cs

tests/
  GasApp.Domain.Tests/
  GasApp.Application.Tests/
  GasApp.API.IntegrationTests/
```

**Dependencias:** API → Application + Infrastructure → Domain (Domain sin referencias externas)

---

## Sprints

### Sprint 1 — COMPLETADO ✅
**Rama:** `main` | **Commits:** `dd02dce`, `c8c59d5`

#### Entidades de dominio
- `AuditableEntity` — base con Id (Guid), CreatedAt, UpdatedAt, DeletedAt, Domain Events
- `User` — factory `User.Create(...)`, roles, soft delete, activo/inactivo
- `UserSession` — refresh tokens, `IsExpired`, `IsRevoked`, `Revoke()`

#### Value Objects
- `Email` — normalización + validación en constructor
- `GeoPoint` — latitud/longitud con validación de rangos

#### Enums
- `UserRole`: Technician, Supervisor, Admin, ClientUser, Auditor
- `InspectionStatus`: Pending, PreCheck, InProgress, TechnicalReview, GeneratingDocs, Completed, RequiresFollowup, Rejected
- `WorkOrderStatus`: Draft, Scheduled, Assigned, InProgress, Completed, Cancelled

#### Servicios de dominio
- `InspectionStateMachine` — FSM estática con transiciones y permisos por rol

#### Excepciones
- `DomainException`, `NotFoundException` (: DomainException), `InvalidStateTransitionException`

#### Application Layer
- **Auth:** LoginCommand, RefreshTokenCommand, LogoutCommand (con handlers y validadores)
- **Users:** CreateUserCommand, UpdateUserCommand, ToggleUserStatusCommand, GetUsersQuery, GetUserByIdQuery
- **Behaviors:** ValidationBehavior (FluentValidation), LoggingBehavior
- **Interfaces:** IJwtTokenService, IPasswordHasher, ICurrentUserService

#### Infrastructure
- `AppDbContext` — enums como string, Owned Entity para Email, query filter soft delete
- Repositorios: UserRepository, UserSessionRepository, UnitOfWork
- `JwtTokenService` — HS256, hash SHA-256 para refresh tokens
- `BcryptPasswordHasher` — work factor 12
- Migración EF Core: `20260623013726_InitialSchema` (tablas `users` y `user_sessions`)

#### API
- `AuthController`: POST /api/v1/auth/login, /refresh, /logout
- `UsersController`: CRUD completo con autorización por rol
- `ExceptionHandlingMiddleware` — manejo global (400/401/404/422/500)
- `CurrentUserMiddleware` — inyecta ICurrentUserService desde claims JWT
- Swagger con Bearer security

#### Tests
- `InspectionStateMachineTests` — 12 tests, **todos en verde** ✅

#### Infraestructura local
- `docker-compose.yml` — PostgreSQL 16 + Redis 7

---

### Sprint 2 — PENDIENTE
**Scope:** Clientes, Contratos, Ubicaciones, Instalaciones, Órdenes de trabajo, Plantillas de checklist

Entidades a implementar:
- `Client` — razón social, NIT, tipo, estado
- `Contract` — cliente, fechas, cobertura, estado
- `Location` — dirección, municipio, departamento, GeoPoint
- `Installation` — tipo de instalación, capacidad, estado
- `WorkOrder` — asignación técnico, fecha programada, estado
- `ChecklistTemplate` + `ChecklistSection` + `ChecklistItem`

---

## Convenciones del proyecto

- Tablas en `snake_case` (EF Core `UseSnakeCaseNamingConvention`)
- Enums almacenados como string en BD
- Soft delete vía `DeletedAt` nullable + query filter automático
- Commands/Queries en carpetas por módulo: `Application/{Modulo}/Commands/{NombreCommand}/`
- Cada Command/Query tiene su Handler y Validator en la misma carpeta
- Errores del dominio siempre via excepciones tipadas (nunca Result<T> en dominio)
- JWT: token crudo nunca persiste — solo hash SHA-256

---

## Cómo correr el proyecto

```bash
# 1. Levantar BD y Redis
docker compose up -d

# 2. Aplicar migraciones
dotnet ef database update --project src/GasApp.Infrastructure --startup-project src/GasApp.API

# 3. Correr la API
dotnet run --project src/GasApp.API

# 4. Swagger en: https://localhost:{puerto}/swagger
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
