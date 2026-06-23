# Plan de ejecución y stack técnico — Sistema de revisiones de gas

## 1. Stack técnico

### Backend API

**ASP.NET Core 8 Web API + C#**

ASP.NET Core 8 ofrece excelente rendimiento, tipado estático con C#, soporte nativo de OpenAPI/Swagger, y un ecosistema maduro para sistemas regulados. La arquitectura Clean Architecture + CQRS con MediatR encaja perfectamente con el dominio complejo de inspecciones (estados, trazabilidad, documentos). C# fuerza contratos explícitos que reducen errores en lógica de negocio crítica.

**ORM: Entity Framework Core 8 + Migraciones EF**

EF Core con Code First y Fluent API para configuración de esquema. Las migraciones son versionadas y reproducibles. Se usa con `DbContext` por módulo de dominio (bounded context).

**Autenticación: JWT + Refresh Tokens (ASP.NET Core Identity + custom JWT service)**

Tokens JWT de 15 minutos, refresh tokens opacos de 30 días almacenados en base de datos. Scopes por rol resueltos en middleware.

**Jobs asíncronos: Hangfire + Redis**

Hangfire es más maduro que Celery para .NET: tiene dashboard de administración integrado, reintentos configurables, jobs recurrentes con cron expressions y soporte de transacciones. Redis como broker y caché.

**Generación de PDFs: QuestPDF**

QuestPDF es la librería moderna de .NET para generación de PDFs mediante una API fluent en C#, sin dependencias nativas externas. Permite diseñar el layout del certificado en código, con soporte de imágenes de evidencias incrustadas.

**Validaciones: FluentValidation**

Validaciones complejas en la capa de Application con FluentValidation, integradas en el pipeline de MediatR como `ValidationBehavior`.

**Logging: Serilog + Seq (desarrollo) / Application Insights (producción)**

Logging estructurado con Serilog, sinks configurables por entorno.

### Base de datos

**PostgreSQL 15+**

JSONB para datos de checklist variables. Full-text search en español. pgcrypto para campos sensibles. Soporte ACID crítico para trazabilidad normativa. Compatible con EF Core sin restricciones.

### Almacenamiento de archivos

**MinIO (self-hosted) o Cloudflare R2**

Compatible con la API de AWS S3. MinIO para soberanía de datos on-premise si el cliente lo requiere. Pre-signed URLs con expiración de 1 hora para acceso seguro.

### App mobile

**React Native + Expo + WatermelonDB**

Un solo codebase para iOS y Android. Expo simplifica cámara, GPS y firma. WatermelonDB para sincronización offline con SQLite local.

Librerías clave:
- `expo-camera` — captura de fotos
- `react-native-signature-canvas` — firma digital
- `expo-location` — geolocalización
- `expo-secure-store` — almacenamiento seguro del token
- `expo-file-system` — archivos locales offline
- `@tanstack/react-query` — sincronización con servidor
- `zustand` — estado global

### App web

**React + Vite + TypeScript**

SPA administrativa sin SSR. Tipos generados automáticamente desde el schema OpenAPI del backend con `openapi-typescript`.

Librerías clave:
- `@tanstack/react-query` — fetching y caché
- `@tanstack/router` — routing tipado
- `shadcn/ui` — componentes
- `react-hook-form` + `zod` — formularios
- `recharts` — gráficos
- `date-fns` — fechas en español

### Deployment

**Docker + Docker Compose (desarrollo) → Railway.app o Render.com (MVP) → AWS São Paulo sa-east-1 (escala)**

| Componente | Container |
|---|---|
| API ASP.NET Core | `gasapp-api` |
| Worker Hangfire | mismo binario, modo worker |
| PostgreSQL | gestionado |
| Redis | gestionado |
| Archivos | MinIO o Cloudflare R2 |
| Web | Cloudflare Pages o Vercel |
| Mobile | EAS Build (Expo) |

### Testing

| Capa | Herramienta |
|---|---|
| Dominio y Application | xUnit + FluentAssertions + NSubstitute |
| Integración API | WebApplicationFactory + Testcontainers (PostgreSQL real) |
| E2E web | Playwright |
| Mobile | Jest + React Native Testing Library |

---

## 2. Arquitectura — Clean Architecture + CQRS

### Principios aplicados

**Single Responsibility:** cada clase tiene una sola razón de cambio. Un `InspectionRepository` solo persiste, un `GenerateCertificateHandler` solo orquesta la generación.

**Open/Closed:** los nuevos tipos de inspección o de evidencia se añaden extendiendo (nuevas implementaciones de `IChecklistRenderer`, nuevo handler), sin modificar los existentes.

**Dependency Inversion:** la capa de Dominio y Application definen interfaces (`IInspectionRepository`, `IPdfGenerator`, `IStorageService`). Infrastructure las implementa. La API consume Application sin conocer Infrastructure.

**CQRS con MediatR:** toda operación de escritura es un `Command` con su `Handler`; toda lectura es una `Query` con su `Handler`. El pipeline de MediatR ejecuta validaciones, logging y auditoría automáticamente en cada operación.

### Diagrama de capas

```
┌──────────────────────────────────────────────────────┐
│                  GasApp.API                          │
│  Controllers, Middleware, Program.cs, DI setup       │
├──────────────────────────────────────────────────────┤
│              GasApp.Application                      │
│  Commands, Queries, Handlers, Validators,            │
│  Behaviors (Validation, Logging, Audit), Interfaces  │
├──────────────────────────────────────────────────────┤
│               GasApp.Domain                          │
│  Entities, Value Objects, Domain Events,             │
│  Repository Interfaces, FSM, Domain Exceptions       │
├──────────────────────────────────────────────────────┤
│            GasApp.Infrastructure                     │
│  EF Core, Repositories, QuestPDF, S3,               │
│  Hangfire Jobs, JWT, Email, Serilog                  │
└──────────────────────────────────────────────────────┘
        ↑ Las dependencias apuntan hacia adentro ↑
```

### Estructura de solución

```
GasApp/
├── src/
│   ├── GasApp.Domain/
│   │   ├── Entities/
│   │   │   ├── Inspections/
│   │   │   │   ├── Inspection.cs
│   │   │   │   ├── WorkOrder.cs
│   │   │   │   └── InspectionStatusHistory.cs
│   │   │   ├── Clients/
│   │   │   │   ├── Client.cs
│   │   │   │   ├── Contract.cs
│   │   │   │   ├── Location.cs
│   │   │   │   └── Installation.cs
│   │   │   ├── Checklists/
│   │   │   │   ├── ChecklistTemplate.cs
│   │   │   │   ├── ChecklistSection.cs
│   │   │   │   ├── ChecklistItem.cs
│   │   │   │   └── ChecklistResponse.cs
│   │   │   ├── Users/
│   │   │   │   └── User.cs
│   │   │   ├── Evidences/
│   │   │   │   ├── Evidence.cs
│   │   │   │   ├── Signature.cs
│   │   │   │   ├── Finding.cs
│   │   │   │   └── FindingFollowup.cs
│   │   │   └── Certificates/
│   │   │       └── Certificate.cs
│   │   ├── ValueObjects/
│   │   │   ├── Email.cs
│   │   │   ├── Nit.cs
│   │   │   ├── GeoPoint.cs
│   │   │   ├── CertificateNumber.cs
│   │   │   └── Money.cs
│   │   ├── Enums/
│   │   │   ├── InspectionStatus.cs
│   │   │   ├── UserRole.cs
│   │   │   ├── FindingSeverity.cs
│   │   │   └── EvidenceType.cs
│   │   ├── Events/                        # Domain Events
│   │   │   ├── InspectionStartedEvent.cs
│   │   │   ├── InspectionCompletedEvent.cs
│   │   │   └── CertificateIssuedEvent.cs
│   │   ├── Repositories/                  # Interfaces (contratos)
│   │   │   ├── IInspectionRepository.cs
│   │   │   ├── IClientRepository.cs
│   │   │   ├── IUserRepository.cs
│   │   │   ├── ICertificateRepository.cs
│   │   │   └── IUnitOfWork.cs
│   │   ├── Services/
│   │   │   └── InspectionStateMachine.cs  # FSM de estados
│   │   └── Exceptions/
│   │       ├── DomainException.cs
│   │       └── InvalidStateTransitionException.cs
│   │
│   ├── GasApp.Application/
│   │   ├── Inspections/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateWorkOrder/
│   │   │   │   │   ├── CreateWorkOrderCommand.cs
│   │   │   │   │   ├── CreateWorkOrderHandler.cs
│   │   │   │   │   └── CreateWorkOrderValidator.cs
│   │   │   │   ├── StartInspection/
│   │   │   │   ├── SubmitChecklist/
│   │   │   │   ├── CompleteInspection/
│   │   │   │   └── TransitionStatus/
│   │   │   └── Queries/
│   │   │       ├── GetInspectionById/
│   │   │       ├── GetTechnicianAgenda/
│   │   │       └── GetInspectionDashboard/
│   │   ├── Clients/
│   │   │   ├── Commands/
│   │   │   └── Queries/
│   │   ├── Certificates/
│   │   │   └── Commands/
│   │   │       └── GenerateCertificate/
│   │   │           ├── GenerateCertificateCommand.cs
│   │   │           └── GenerateCertificateHandler.cs
│   │   ├── Evidences/
│   │   │   └── Commands/
│   │   ├── Common/
│   │   │   ├── Behaviors/
│   │   │   │   ├── ValidationBehavior.cs   # FluentValidation pipeline
│   │   │   │   ├── LoggingBehavior.cs
│   │   │   │   └── AuditBehavior.cs
│   │   │   └── Interfaces/
│   │   │       ├── IStorageService.cs
│   │   │       ├── IEmailService.cs
│   │   │       ├── IPdfGenerator.cs
│   │   │       ├── IJobScheduler.cs
│   │   │       └── ICurrentUserService.cs
│   │   └── DependencyInjection.cs
│   │
│   ├── GasApp.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/            # EF Fluent API
│   │   │   │   ├── InspectionConfiguration.cs
│   │   │   │   ├── ClientConfiguration.cs
│   │   │   │   └── CertificateConfiguration.cs
│   │   │   ├── Repositories/
│   │   │   │   ├── InspectionRepository.cs
│   │   │   │   ├── ClientRepository.cs
│   │   │   │   └── CertificateRepository.cs
│   │   │   └── Migrations/
│   │   ├── Storage/
│   │   │   └── S3StorageService.cs
│   │   ├── Email/
│   │   │   └── SmtpEmailService.cs
│   │   ├── Pdf/
│   │   │   ├── QuestPdfGenerator.cs
│   │   │   └── Templates/
│   │   │       ├── CertificateDocument.cs
│   │   │       └── InspectionReportDocument.cs
│   │   ├── Jobs/
│   │   │   ├── HangfireJobScheduler.cs
│   │   │   └── RecurringJobs/
│   │   │       ├── ExpirationReminderJob.cs
│   │   │       └── CertificateGenerationJob.cs
│   │   ├── Auth/
│   │   │   └── JwtTokenService.cs
│   │   └── DependencyInjection.cs
│   │
│   └── GasApp.API/
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── InspectionsController.cs
│       │   ├── ClientsController.cs
│       │   ├── ChecklistsController.cs
│       │   ├── EvidencesController.cs
│       │   ├── CertificatesController.cs
│       │   └── ReportsController.cs
│       ├── Middleware/
│       │   ├── ExceptionHandlingMiddleware.cs
│       │   └── CurrentUserMiddleware.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── Program.cs
│
├── tests/
│   ├── GasApp.Domain.Tests/
│   ├── GasApp.Application.Tests/
│   └── GasApp.API.IntegrationTests/        # Testcontainers + WebApplicationFactory
│
├── gasapp-web/                              # React + Vite
├── gasapp-mobile/                           # React Native + Expo
├── GasApp.sln
├── docker-compose.yml
└── docker-compose.prod.yml
```

### Patrones clave del dominio

**Entidad base con auditoría:**
```csharp
public abstract class AuditableEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
```

**Value Object (ejemplo Email):**
```csharp
public record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
            throw new DomainException("Email inválido");
        Value = value.ToLowerInvariant().Trim();
    }
}
```

**FSM de estados de inspección:**
```csharp
public class InspectionStateMachine
{
    private static readonly Dictionary<InspectionStatus, InspectionStatus[]> _allowedTransitions = new()
    {
        [InspectionStatus.Pending]          = [InspectionStatus.PreCheck],
        [InspectionStatus.PreCheck]         = [InspectionStatus.InProgress],
        [InspectionStatus.InProgress]       = [InspectionStatus.TechnicalReview],
        [InspectionStatus.TechnicalReview]  = [InspectionStatus.GeneratingDocs, InspectionStatus.Rejected],
        [InspectionStatus.GeneratingDocs]   = [InspectionStatus.Completed],
    };

    public static void Transition(Inspection inspection, InspectionStatus to, UserRole role)
    {
        if (!_allowedTransitions.TryGetValue(inspection.Status, out var allowed) || !allowed.Contains(to))
            throw new InvalidStateTransitionException(inspection.Status, to);

        ValidateRolePermission(inspection.Status, to, role);
        inspection.ApplyTransition(to);
    }
}
```

**Command + Handler con MediatR:**
```csharp
// Command
public record StartInspectionCommand(Guid InspectionId, Guid TechnicianId) : IRequest<InspectionDto>;

// Handler
public class StartInspectionHandler(
    IInspectionRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<StartInspectionCommand, InspectionDto>
{
    public async Task<InspectionDto> Handle(StartInspectionCommand cmd, CancellationToken ct)
    {
        var inspection = await repository.GetByIdAsync(cmd.InspectionId, ct)
            ?? throw new NotFoundException(nameof(Inspection), cmd.InspectionId);

        InspectionStateMachine.Transition(inspection, InspectionStatus.InProgress, UserRole.Technician);
        inspection.AssignTechnician(cmd.TechnicianId);
        inspection.AddDomainEvent(new InspectionStartedEvent(inspection.Id));

        await unitOfWork.SaveChangesAsync(ct);
        return InspectionDto.From(inspection);
    }
}
```

**Validator con FluentValidation:**
```csharp
public class StartInspectionValidator : AbstractValidator<StartInspectionCommand>
{
    public StartInspectionValidator()
    {
        RuleFor(x => x.InspectionId).NotEmpty();
        RuleFor(x => x.TechnicianId).NotEmpty();
    }
}
```

**ValidationBehavior en el pipeline de MediatR:**
```csharp
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var failures = validators
            .SelectMany(v => v.Validate(request).Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
```

---

## 3. Modelo de datos

### Dominio de identidad

```sql
users
  id              uuid PK
  email           text UNIQUE NOT NULL
  hashed_password text NOT NULL
  first_name      text
  last_name       text
  phone           text
  role            enum(technician, supervisor, admin, client_user, auditor)
  is_active       bool DEFAULT true
  created_at      timestamptz
  updated_at      timestamptz
  deleted_at      timestamptz    -- soft delete

user_sessions
  id                  uuid PK
  user_id             uuid FK users
  refresh_token_hash  text
  device_info         jsonb
  expires_at          timestamptz
  revoked_at          timestamptz
```

### Dominio de clientes

```sql
clients
  id                   uuid PK
  business_name        text NOT NULL
  nit                  text UNIQUE NOT NULL
  legal_representative text
  contact_email        text
  contact_phone        text
  address              text
  city                 text
  client_type          enum(residential, commercial, industrial)
  is_active            bool DEFAULT true

contracts
  id                   uuid PK
  client_id            uuid FK clients
  contract_number      text UNIQUE NOT NULL
  start_date           date
  end_date             date
  inspection_frequency enum(monthly, quarterly, semi_annual, annual)
  status               enum(active, suspended, terminated)

locations
  id              uuid PK
  client_id       uuid FK clients
  contract_id     uuid FK contracts
  name            text
  address         text
  lat             decimal(9,6)
  lng             decimal(9,6)
  contact_name    text
  contact_phone   text
  is_active       bool DEFAULT true

installations
  id                  uuid PK
  location_id         uuid FK locations
  installation_code   text
  appliance_type      enum(boiler, stove, heater, industrial_burner, other)
  brand               text
  model               text
  serial_number       text
  installation_date   date
  last_inspection_id  uuid FK inspections NULLABLE
  metadata            jsonb
```

### Dominio de inspecciones

```sql
inspection_types
  id                    uuid PK
  name                  text NOT NULL
  frequency_days        int
  normative_reference   text
  checklist_template_id uuid FK checklist_templates
  is_active             bool DEFAULT true

work_orders
  id                      uuid PK
  order_number            text UNIQUE NOT NULL
  client_id               uuid FK clients
  location_id             uuid FK locations
  inspection_type_id      uuid FK inspection_types
  status                  enum(draft, scheduled, assigned, in_progress, completed, cancelled)
  priority                enum(low, normal, high, urgent)
  scheduled_date          date
  assigned_technician_id  uuid FK users NULLABLE
  supervisor_id           uuid FK users NULLABLE
  notes                   text
  created_by              uuid FK users
  created_at              timestamptz
  updated_at              timestamptz

inspections
  id                uuid PK
  work_order_id     uuid FK work_orders UNIQUE
  technician_id     uuid FK users
  status            enum(pending, pre_check, in_progress, technical_review,
                         generating_docs, completed, requires_followup, rejected)
  started_at        timestamptz
  completed_at      timestamptz
  geo_start_lat     decimal(9,6)
  geo_start_lng     decimal(9,6)
  geo_end_lat       decimal(9,6)
  geo_end_lng       decimal(9,6)
  overall_result    enum(approved, approved_with_observations,
                         rejected, requires_immediate_action) NULLABLE
  reviewed_by       uuid FK users NULLABLE
  reviewed_at       timestamptz NULLABLE

inspection_status_history
  id             uuid PK
  inspection_id  uuid FK inspections
  from_status    text
  to_status      text
  changed_by     uuid FK users
  reason         text
  changed_at     timestamptz
```

### Dominio de checklists

```sql
checklist_templates
  id                  uuid PK
  name                text NOT NULL
  version             int NOT NULL
  inspection_type_id  uuid FK inspection_types
  is_active           bool DEFAULT true
  is_current_version  bool DEFAULT true

checklist_sections
  id           uuid PK
  template_id  uuid FK checklist_templates
  name         text
  order_index  int
  is_required  bool DEFAULT true

checklist_items
  id                  uuid PK
  section_id          uuid FK checklist_sections
  code                text
  description         text NOT NULL
  response_type       enum(boolean, multiple_choice, numeric, text, photo_required)
  options             jsonb
  is_critical         bool DEFAULT false
  normative_reference text
  order_index         int

checklist_responses
  id                    uuid PK
  inspection_id         uuid FK inspections
  item_id               uuid FK checklist_items
  response_value        jsonb NOT NULL
  observation           text
  responded_at          timestamptz
  responded_by          uuid FK users
  local_id              text     -- UUID generado offline
  synced_at             timestamptz
```

### Dominio de evidencias

```sql
evidences
  id                      uuid PK
  inspection_id           uuid FK inspections
  checklist_response_id   uuid FK checklist_responses NULLABLE
  finding_id              uuid FK findings NULLABLE
  evidence_type           enum(photo, video, document, voice_note)
  file_key                text     -- key en S3/MinIO
  file_name_original      text
  file_size               bigint
  mime_type               text
  metadata                jsonb    -- EXIF, duración, etc.
  geo_lat                 decimal(9,6)
  geo_lng                 decimal(9,6)
  captured_at             timestamptz
  uploaded_by             uuid FK users
  local_id                text
  synced_at               timestamptz

signatures
  id                 uuid PK
  inspection_id      uuid FK inspections
  signer_type        enum(client, technician, supervisor, witness)
  signer_name        text
  signer_id_number   text
  signature_file_key text
  geo_lat            decimal(9,6)
  geo_lng            decimal(9,6)
  signed_at          timestamptz
  ip_address         text

findings
  id                         uuid PK
  inspection_id              uuid FK inspections
  installation_id            uuid FK installations NULLABLE
  title                      text
  description                text
  finding_type               enum(non_conformity, observation, improvement, critical_risk)
  severity                   enum(critical, major, minor, informational)
  normative_reference        text
  immediate_action_required  bool DEFAULT false
  recommended_action         text
  due_date                   date
  status                     enum(open, in_progress, resolved, accepted_risk, closed)
  created_at                 timestamptz

finding_followups
  id                uuid PK
  finding_id        uuid FK findings
  notes             text
  status_changed_to text
  updated_by        uuid FK users
  created_at        timestamptz
```

### Dominio de certificados y auditoría

```sql
certificates
  id                  uuid PK
  inspection_id       uuid FK inspections UNIQUE
  certificate_number  text UNIQUE NOT NULL    -- ej: 2025061200043
  version             int DEFAULT 1
  certificate_type    enum(inspection, conformity, non_conformity)
  validity_months     int
  valid_from          date
  valid_until         date
  file_key            text
  file_hash           text                    -- SHA-256
  qr_code_data        text
  issued_at           timestamptz
  issued_by           uuid FK users
  status              enum(draft, issued, revoked)
  revoked_at          timestamptz NULLABLE
  revocation_reason   text NULLABLE

audit_log
  id          bigserial PK                    -- bigint por volumen alto
  table_name  text NOT NULL
  record_id   text NOT NULL
  action      enum(INSERT, UPDATE, DELETE)
  old_values  jsonb
  new_values  jsonb
  changed_by  uuid FK users
  changed_at  timestamptz
  ip_address  text
  user_agent  text

notifications
  id        uuid PK
  user_id   uuid FK users
  type      enum(inspection_assigned, reminder, certificate_ready, finding_due, system_alert)
  title     text
  body      text
  data      jsonb
  channel   enum(push, email, sms, in_app)
  status    enum(pending, sent, delivered, failed, read)
  sent_at   timestamptz
  read_at   timestamptz
```

---

## 4. Roadmap por sprints

### Sprint 0 — Descubrimiento y setup (semanas 1–2)

**Objetivo:** validar el proceso real y dejar el entorno listo para desarrollo.

**Actividades:**
- Taller con el cliente: validar flujos operativos y checklist normativo (NTC 3631, RETIE)
- Definir y aprobar la plantilla exacta del certificado/acta
- Identificar integraciones requeridas (email, SMS, sistemas ERP existentes)
- Inventariar dispositivos móviles de los técnicos (Android/iOS, versión)
- Definir política de retención de datos y soberanía (on-premise vs cloud)
- Setup de repositorios Git con estructura de solución .NET (Clean Architecture)
- `docker-compose.yml` con PostgreSQL, Redis y MinIO para desarrollo local
- CI/CD básico con GitHub Actions (build, test, lint)
- Primera migración EF Core con tablas de identity

**Entregables:**
- Casos de uso validados con el cliente
- Plantilla del certificado aprobada
- Entorno local corriendo en menos de 10 minutos con `docker-compose up`

**Criterios de éxito:**
- Cliente firma la plantilla del certificado
- Todo el equipo puede clonar y correr el proyecto local sin fricción

---

### Sprint 1 — Dominio base y autenticación (semanas 3–4)

**Objetivo:** construir el núcleo del dominio y la seguridad.

**Backend:**
- Entidades base: `User`, `Role`, Value Objects (`Email`, `Nit`, `GeoPoint`)
- `InspectionStateMachine` en el dominio con todas las transiciones válidas
- Módulo `auth`: login, registro, refresh token, logout, cambio de contraseña
- Módulo `users`: CRUD con control de roles y permisos
- Pipeline de MediatR con `ValidationBehavior`, `LoggingBehavior` y `AuditBehavior`
- Middleware de manejo global de excepciones con respuestas consistentes
- Middleware de auditoría (registra IP y usuario en cada operación sensible)

**Entregables:**
- Endpoints de auth funcionales con tests de integración
- Documentación Swagger accesible en `/swagger`
- Roles bloqueando correctamente el acceso a endpoints no autorizados

---

### Sprint 2 — Clientes, contratos, sedes e inspecciones (semanas 5–6)

**Objetivo:** habilitar la gestión operativa central del backend.

**Backend:**
- Entidades y repositorios: `Client`, `Contract`, `Location`, `Installation`
- CRUD completo de clientes, contratos, sedes e instalaciones
- Entidades: `InspectionType`, `WorkOrder`, `Inspection`
- Commands: `CreateWorkOrder`, `AssignTechnician`, `TransitionStatus`
- Queries: `GetWorkOrderList` (con filtros y paginación), `GetTechnicianAgenda`
- Entidades y templates de checklist: `ChecklistTemplate`, `ChecklistSection`, `ChecklistItem`
- Endpoints de sincronización mobile: `POST /sync/checklist-responses` (batch)

**Entregables:**
- Se puede crear cliente → contrato → sede → orden de trabajo → asignar técnico vía API
- Colección Postman/Insomnia completa para todos los módulos

---

### Sprint 3 — Web admin: autenticación, clientes y agenda (semanas 7–8)

**Objetivo:** habilitar la operación básica de oficina.

**Web:**
- Setup Vite + React + TypeScript + shadcn/ui + TanStack Router/Query
- Login, manejo de sesión con refresh automático de token
- Layout principal: sidebar de navegación por rol, top bar
- Módulo Clientes: listado con filtros, detalle, crear/editar, contratos y sedes
- Módulo Órdenes de Trabajo: listado, filtros por estado/técnico/fecha, crear orden
- Vista de Agenda/Calendario: asignación de técnicos a órdenes por fecha
- Dashboard básico: contadores por estado, inspecciones del día, alertas
- Gestión de usuarios y roles (solo rol admin)
- Tipos generados automáticamente desde el OpenAPI schema del backend

**Entregables:**
- Web app desplegada en staging
- Supervisor puede ver la agenda, crear órdenes y asignar técnicos sin manual

---

### Sprint 4 — Mobile: setup y agenda offline (semanas 9–10)

**Objetivo:** técnico puede ver su agenda y abrir inspecciones sin conexión.

**Mobile:**
- Setup Expo + Expo Router + WatermelonDB con esquema local
- Login mobile con token almacenado en `expo-secure-store`
- Pantalla de agenda del técnico: inspecciones del día asignadas
- Detalle de orden de trabajo: datos del cliente, sede e instalaciones
- Descarga (precarga) de la orden para trabajo offline en WatermelonDB
- Cambio de estado de inspección: `Assigned → InProgress`
- Indicador de estado de conectividad en tiempo real
- Servicio de sincronización básico: push de cambios de estado al reconectar

**Entregables:**
- APK de prueba instalable en dispositivos de los técnicos
- El técnico ve su agenda diaria y puede abrir una inspección sin internet

---

### Sprint 5 — Mobile: checklist offline y sincronización robusta (semanas 11–12)

**Objetivo:** técnico puede completar un checklist completo sin conexión y sincronizar sin pérdida de datos.

**Mobile:**
- Pantalla de checklist: renderizado dinámico según `response_type` del ítem
- Guardado de respuestas en WatermelonDB con `is_synced: false` y `local_id` (UUID v4)
- Sincronización en batch: `POST /sync/checklist-responses` con todas las respuestas pendientes
- Manejo de conflictos: last-write-wins por `responded_at`
- Retry exponencial para sincronización fallida (máximo 5 intentos)
- `SyncStatusBar`: indicador visual de pendientes, sincronizando, sincronizado
- Tests en dispositivos físicos Android e iOS con simulación de pérdida de red

**Entregables:**
- Técnico completa un checklist completo de 30 ítems sin conexión
- Al reconectar, sincroniza en menos de 10 segundos sin pérdida de datos

---

### Sprint 6 — Evidencias y firma (semanas 13–14)

**Objetivo:** captura de fotos, hallazgos y firma digital en campo.

**Mobile:**
- Captura de fotos con `expo-camera` + compresión automática a máx 500 KB
- Asociación de fotos a ítems del checklist y a hallazgos
- Almacenamiento local de evidencias offline en `expo-file-system`
- Upload a S3/MinIO al sincronizar con retry exponencial
- Módulo de hallazgos: registrar desde mobile, asociar a ítem de checklist, asignar severidad
- Firma digital del cliente: `react-native-signature-canvas` + nombre + número de documento

**Backend:**
- `POST /evidences/upload` con pre-signed URL para subida directa a S3
- `POST /signatures` para almacenar firma, metadatos y geolocalización
- `POST /findings` para registrar hallazgos desde mobile
- Sincronización de evidencias: verificación de integridad por hash

**Entregables:**
- Flujo completo campo: fotos + hallazgos + firma sincronizados al servidor
- Evidencias almacenadas en S3 accesibles desde la web con URLs pre-signed

---

### Sprint 7 — Certificados, PDF y QR (semanas 15–16)

**Objetivo:** cierre documental completo del ciclo de inspección.

**Backend:**
- `GenerateCertificateCommand` con handler orquestador
- `CertificateGenerationJob` en Hangfire: carga datos, genera PDF, sube a S3, actualiza estado
- `CertificateDocument.cs` con QuestPDF: template del certificado con fotos de evidencias incrustadas
- Numeración con sequence de PostgreSQL: `{AÑO}{MES}{SECUENCIA_5DÍGITOS}`
- Hash SHA-256 del PDF para integridad
- Generación de datos QR con URL de verificación pública
- Endpoint público `GET /verify/{certificateNumber}` sin autenticación
- `SendCertificateEmailJob`: envío del PDF al cliente por email
- Versionado: si se reedita, `version++` y se revoca el anterior

**Web:**
- Vista de detalle de inspección con descarga y previsualización del certificado
- Botón de aprobación del supervisor con transición `TechnicalReview → GeneratingDocs`

**Entregables:**
- Flujo end-to-end: supervisor aprueba → PDF generado → email al cliente en menos de 30 segundos
- QR de verificación funcional apuntando a datos del certificado

---

### Sprint 8 — Reportes, dashboard y portal cliente (semanas 17–18)

**Objetivo:** control operativo completo y analítica básica.

**Web:**
- Dashboard ejecutivo con Recharts: inspecciones por período, técnico, sede, resultado
- Reporte de cumplimiento: instalaciones vencidas o próximas a vencer
- Reporte de hallazgos abiertos por cliente y severidad
- Exportación de reportes a Excel (EPPlus en backend)
- Portal del cliente: vista de sus sedes, historial de inspecciones, descarga de certificados
- Historial completo de inspección con trazabilidad de estados

**Backend + Mobile:**
- Notificaciones push con Expo Notifications
- Jobs recurrentes Hangfire: recordatorios de vencimiento próximo (Celery beat equivalente)
- Mejoras de UX mobile basadas en feedback de los técnicos del Sprint 5-6

**Entregables:**
- Supervisor ve en un vistazo el estado operativo del día
- Cliente descarga sus certificados sin contactar a la empresa

---

### Sprint 9 — QA, seguridad, despliegue y documentación (semanas 19–20)

**Objetivo:** salida controlada a producción.

**QA y seguridad:**
- Pruebas de carga con k6: 50 técnicos sincronizando simultáneamente
- Revisión OWASP Top 10: inyección SQL, manejo de tokens, exposición de datos
- Rate limiting: login (5 req/min/IP), sync mobile (20 req/min/usuario)
- Cifrado en tránsito (HTTPS obligatorio) y en reposo (campos sensibles con pgcrypto)
- Pruebas de sincronización offline en condiciones de red deficiente (3G simulado)
- UAT con técnicos reales en campo

**Despliegue:**
- Setup de entorno de producción (Railway o AWS sa-east-1)
- Backups automáticos de PostgreSQL con retención de 30 días
- Monitoreo: Serilog → Application Insights o Seq, alertas por caída del servicio
- Dashboard Hangfire protegido por autenticación en producción
- Pipeline CI/CD: deploy automático a staging en push a `develop`, manual a producción
- Publicación en Google Play (TestFlight para iOS si aplica)

**Documentación:**
- Manual de usuario para técnicos (PDF + video corto de capacitación)
- Manual de administración del sistema
- Runbook de operaciones: backup, restore, escalado, rotación de secretos
- Capacitación al equipo del cliente

**Criterios de éxito:**
- Sistema aguanta 50 usuarios concurrentes sin degradación perceptible
- No hay vulnerabilidades OWASP críticas ni altas
- La app está publicada y disponible para descarga en Google Play
- El cliente firma el acta de aceptación del sistema

---

## 5. Decisiones técnicas clave

### Sincronización offline

**Modelo: Pull-before-work + Push-on-complete con reconciliación por timestamp**

Al iniciar sesión con conectividad, la app descarga en background las órdenes del día completas (cliente, sede, instalaciones, checklist). Durante la ejecución todo se guarda en WatermelonDB con `is_synced: false` y `local_timestamp`. Al recuperar conectividad:

1. Sube respuestas del checklist en batch
2. Sube evidencias una a una con retry exponencial
3. Actualiza el estado de la inspección
4. Descarga actualizaciones del servidor (notas del supervisor)

**Conflictos:** Last-write-wins por `responded_at`. Una inspección es exclusiva del técnico asignado en el MVP; el conflicto real es improbable. Se loguea todo en `sync_conflicts` para revisión.

**Identificadores offline:** UUIDs v4 generados en el cliente en el campo `local_id`. Al sincronizar, el servidor responde con el ID definitivo de la base de datos.

### Generación de certificados

Hangfire job asíncrono invocado al transicionar a `GeneratingDocs`. El job carga todos los datos, genera el PDF con QuestPDF (incluyendo fotos de evidencias), calcula SHA-256, sube a S3, genera QR, persiste en `certificates` y transiciona a `Completed`. El endpoint público de verificación no requiere autenticación.

### Seguridad

- JWT 15 min + refresh token opaco 30 días en tabla `user_sessions`
- Mobile: token en `expo-secure-store` (Keychain/Keystore)
- Web: access token en memoria, refresh token en httpOnly cookie
- URLs de S3 como pre-signed URLs con expiración de 1 hora
- Triggers de PostgreSQL en tablas críticas para `audit_log` inmutable
- Rate limiting con middleware de ASP.NET Core + Redis

---

## 6. Convenciones y estándares

### API REST

- Recursos en plural con kebab-case en rutas: `/api/v1/work-orders`, `/api/v1/clients/{id}/locations`
- Versionado en path: `/api/v1/`
- Paginación consistente en todas las listas:
  ```json
  { "items": [...], "total": 150, "page": 1, "pageSize": 20, "pages": 8 }
  ```
- Errores con estructura consistente:
  ```json
  { "errorCode": "INSPECTION_INVALID_TRANSITION", "message": "...", "details": {} }
  ```
- Fechas en ISO 8601 UTC en la API; conversión a UTC-5 (Colombia) en el frontend
- Propiedades JSON en camelCase

### Código .NET

- Formato con `dotnet format` + EditorConfig
- Nullable reference types habilitado en todos los proyectos (`<Nullable>enable</Nullable>`)
- Commits en inglés, conventional commits: `feat:`, `fix:`, `chore:`, `docs:`
- Nombres de tablas en snake_case plural en la base de datos (configurado en EF Fluent API)
- Nombres de columnas en snake_case
- `async/await` en todos los métodos de I/O; nunca `.Result` ni `.Wait()`
- Records para Commands, Queries y DTOs (inmutabilidad)

### Código frontend

- TypeScript estricto (`strict: true` en tsconfig)
- ESLint + Prettier
- Componentes en PascalCase, hooks con prefijo `use`
- No se hace fetch directamente en componentes, siempre via hooks de TanStack Query
- Tipos generados desde el schema OpenAPI del backend con `openapi-typescript`

### Base de datos

- Toda tabla tiene: `id` (uuid), `created_at`, `updated_at`
- Soft delete con `deleted_at` nullable en entidades principales
- Índices en todas las FK y columnas de filtros frecuentes
- Migrations EF Core revisadas manualmente antes de commitear
- Nunca modificar una migration ya desplegada en producción; siempre crear una nueva

### Git

- Branch principal: `main` (producción)
- Branch de desarrollo: `develop`
- Feature branches: `feature/{modulo}-{descripcion-corta}`
- Hotfix: `hotfix/{descripcion}`
- PRs obligatorios hacia `develop` con review de al menos 1 desarrollador
- Tags en `main` con versión semántica: `v1.0.0`, `v1.1.0`

### Entornos

| Variable | Desarrollo | Staging | Producción |
|---|---|---|---|
| ASPNETCORE_ENVIRONMENT | Development | Staging | Production |
| Serilog Level | Debug | Information | Warning |
| JWT expiry | 60 min | 15 min | 15 min |
| PDF Watermark | "BORRADOR" | "PRUEBA" | — |
| Hangfire Dashboard | público | autenticado | autenticado |

---

## 7. Resumen de sprints y recursos

| Sprint | Semanas | Foco principal | Equipo |
|---|---|---|---|
| 0 | 1–2 | Descubrimiento + setup | 1 tech lead + cliente |
| 1 | 3–4 | Dominio base + auth backend | 2 backend |
| 2 | 5–6 | Clientes, órdenes, checklists API | 2 backend |
| 3 | 7–8 | Web admin: login, clientes, agenda | 1–2 frontend |
| 4 | 9–10 | Mobile: setup + agenda offline | 1–2 mobile |
| 5 | 11–12 | Mobile: checklist offline + sync robusta | 1–2 mobile |
| 6 | 13–14 | Evidencias + firma (mobile + API) | 2 fullstack |
| 7 | 15–16 | Certificados + PDF + QR | 2 fullstack |
| 8 | 17–18 | Reportes + dashboard + portal cliente | 1–2 frontend |
| 9 | 19–20 | QA, seguridad, despliegue, docs | Todo el equipo |

**Duración total: 20 semanas (5 meses) con un equipo de 3–4 desarrolladores.**

MVP funcional (Sprints 0–7) disponible en la semana 16, con operación real posible antes de terminar los reportes avanzados.

---

## 8. Archivos críticos para arrancar

| Archivo | Por qué es crítico |
|---|---|
| `GasApp.Domain/Services/InspectionStateMachine.cs` | Núcleo del dominio; errores aquí se propagan a toda la lógica de negocio |
| `GasApp.Domain/Entities/Inspections/Inspection.cs` | Entidad central; define invariantes del negocio |
| `GasApp.Infrastructure/Persistence/AppDbContext.cs` | Configuración EF Core; base de todo el acceso a datos |
| `GasApp.Infrastructure/Persistence/Migrations/0001_InitialSchema.cs` | Cambiar el esquema con datos en producción tiene costo alto |
| `GasApp.Application/Common/Behaviors/ValidationBehavior.cs` | Garantiza que ningún Command llegue sin validar |
| `GasApp.API/Program.cs` | Configuración del DI container; conecta todas las capas |
| `gasapp-mobile/src/database/schema.ts` | Define qué datos viven offline; debe estar alineado con el backend desde el día uno |
| `GasApp.Infrastructure/Pdf/Templates/CertificateDocument.cs` | Entregable legal más crítico; requiere aprobación del cliente antes de codificar |
