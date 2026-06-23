# Plan de trabajo para sistema de revisiones de gas

## Resumen ejecutivo del sistema

Propongo una plataforma integral para revisiones de gas compuesta por una app mobile híbrida para técnicos, una app web para operación y administración, y una API central con base de datos, repositorio de evidencias y trazabilidad completa. El diseño debe asumir un flujo regulado donde la inspección, el certificado y la trazabilidad documental son críticos, porque la revisión periódica de gas en Colombia la realiza un organismo de inspección acreditado y termina en un certificado de conformidad o informe equivalente entregable al usuario. [grupovanti](https://www.grupovanti.com/rpo)

A nivel de producto, el sistema debe resolver el control operativo extremo a extremo: planificación, ejecución en campo, validación, generación documental y seguimiento histórico. También debe contemplar necesidades reales del negocio como inspecciones con evidencias fotográficas, firma, sincronización diferida y control de estados, porque este tipo de soluciones suele operar en entornos con conectividad variable y fuerte exigencia de auditoría. [play.google](https://play.google.com/store/apps/details?id=com.movilixa.inspecciona&hl=es)

## Roles y usuarios

Los roles principales deben modelarse por responsabilidad operativa y nivel de acceso, no solo por tipo de dispositivo. En una empresa de revisiones de gas normalmente habrá técnico/inspector, supervisor, administrador operativo, cliente solicitante y un rol de consulta o auditoría para revisar trazabilidad y cumplimiento. [ccconsumidores.org](https://ccconsumidores.org.co/sobre-la-revision-periodica-de-las-instalaciones-internas-de-gas/)

- **Técnico / inspector**: en mobile inicia, ejecuta y cierra inspecciones, registra checklist, hallazgos, fotos, firma y observaciones; en web puede consultar agenda, historiales propios y documentos pendientes; necesita ver orden, ubicación, datos del inmueble, checklist, evidencias y estado de sincronización.
- **Supervisor**: en web reasigna trabajos, valida resultados, revisa inconsistencias, aprueba cierres y monitorea productividad; en mobile normalmente solo consulta o valida casos puntuales; necesita revisar estados, alertas, hallazgos críticos, cumplimiento de SLA y documentación.
- **Administrador**: en web configura usuarios, catálogos, parámetros, permisos, plantillas y reportes; en mobile no suele operar; necesita gestionar empresas, sedes, roles, tipos de inspección, plantillas y reglas de negocio.
- **Cliente o empresa solicitante**: en web o portal consulta programación, estado de revisión, certificados e historial; en mobile puede recibir notificaciones y firmar conformidad si aplica; necesita ver citas, documentos, resultados y vencimientos.
- **Auditor / consulta**: en web accede a trazabilidad, bitácoras y evidencias con permisos restringidos; en mobile normalmente no participa; necesita evidencia de cambios, firmas, tiempos, responsables y versiones.

## Módulos funcionales

Los módulos deben organizarse por dominio de negocio y soporte transversal. Para este caso, conviene separar módulos core de inspección, módulos administrativos y módulos de cumplimiento y trazabilidad. [bizzmine](https://www.bizzmine.com/es/industrias/petroleo-y-gas/)

- Identidad, autenticación y permisos.
- Gestión de clientes y contratos o cuentas.
- Gestión de sedes, inmuebles e instalaciones.
- Programación y asignación de inspecciones.
- Órdenes de trabajo y agenda.
- Ejecución de inspección y checklist técnico.
- Captura de evidencias, fotos, archivos, firmas y geolocalización.
- Hallazgos, no conformidades y acciones correctivas.
- Generación de certificado, acta o informe.
- Historial, trazabilidad y auditoría.
- Notificaciones y recordatorios.
- Reportes operativos y cumplimiento.
- Catálogos técnicos, parámetros y plantillas.
- Sincronización offline/online.
- Administración documental y retención.

## Funcionalidades por plataforma

### App mobile híbrida

La app mobile debe estar optimizada para campo, velocidad y resiliencia offline. Su foco es que el técnico pueda terminar una inspección sin depender de conectividad constante, algo habitual en soluciones de inspección profesional. [viafirma](https://www.viafirma.com/es/faqs/guia-de-uso-offline/)

- Ver agenda asignada y detalle de inspecciones.
- Navegar a la dirección y consultar datos del sitio.
- Iniciar, pausar y cerrar inspección.
- Diligenciar checklist técnico por pasos.
- Registrar hallazgos, observaciones y no conformidades.
- Capturar fotos, videos o archivos soportes.
- Registrar firma del cliente o responsable.
- Tomar geolocalización y sello de tiempo.
- Trabajar offline y sincronizar después.
- Ver estados de sincronización y conflictos.
- Consultar historial de inspecciones propias.
- Descargar formularios o plantillas asignadas.
- Generar cierre preliminar o borrador de informe.

### App web administrativa

La app web debe ser el centro operativo y administrativo. Allí se controlan los recursos, la planeación, el seguimiento y el análisis de cumplimiento. [grupovanti](https://www.grupovanti.com/rpo)

- Crear y administrar clientes, sedes e instalaciones.
- Programar revisiones y asignar técnicos.
- Visualizar tablero de estados y cargas de trabajo.
- Revisar checklist completados y evidencias.
- Aprobar o rechazar inspecciones con observaciones.
- Generar certificados, informes y PDFs.
- Administrar catálogos, parámetros y plantillas.
- Consultar históricos por cliente, sede, técnico o estado.
- Gestionar usuarios, roles y permisos.
- Configurar notificaciones, recordatorios y vencimientos.
- Exportar reportes operativos y de cumplimiento.
- Revisar trazabilidad y auditoría de cambios.

### API backend

La API debe ser la capa central de negocio, seguridad e integración. Debe exponer el dominio de inspecciones y proteger consistencia, permisos y trazabilidad. [bizzmine](https://www.bizzmine.com/es/industrias/petroleo-y-gas/)

- Autenticación y autorización.
- Gestión de usuarios, roles y permisos.
- CRUD de clientes, sedes, inspecciones y checklist.
- Sincronización mobile/web con control de versiones.
- Carga y metadatos de evidencias.
- Gestión de estados y transiciones.
- Generación y consulta de documentos.
- Registro de logs y auditoría.
- Notificaciones y eventos.
- Integraciones futuras con correo, SMS, firma externa o ERP.
- Consultas para dashboards y reportes.

## Flujo operativo del negocio

El flujo debe pensarse como una cadena controlada por estados, desde la creación de la necesidad hasta el cierre documental. En este tipo de negocio, la inspección debe quedar soportada por evidencia suficiente para demostrar cumplimiento y entregar certificado al usuario. [ccconsumidores.org](https://ccconsumidores.org.co/sobre-la-revision-periodica-de-las-instalaciones-internas-de-gas/)

1. **Alta del cliente.**  
   Se registra la empresa o usuario final, datos de contacto, ubicación, sedes e instalaciones a revisar.

2. **Creación de la orden.**  
   Se genera una solicitud u orden de revisión con motivo, tipo de servicio, fecha objetivo y alcance técnico.

3. **Programación.**  
   La operación asigna técnico, ventana de atención, prioridad y condiciones especiales.

4. **Prechequeo.**  
   Se valida información mínima, disponibilidad, documentación previa y estado del equipo de trabajo.

5. **Ejecución en campo.**  
   El técnico recibe la orden en mobile, verifica identidad del sitio y ejecuta el checklist técnico.

6. **Captura de evidencias.**  
   Se adjuntan fotos, observaciones, posibles hallazgos, ubicación y firma del cliente o responsable.

7. **Validación técnica.**  
   Si el flujo lo requiere, un supervisor revisa consistencia, observaciones críticas y completitud.

8. **Generación documental.**  
   Se emite certificado, informe o acta de inspección con número único y metadatos de trazabilidad.

9. **Cierre.**  
   La inspección se marca como finalizada y se actualiza el historial del inmueble y del cliente.

10. **Seguimiento.**  
   Quedan alertas de vencimiento, historial consultable y trazabilidad de quién hizo qué y cuándo.

## Arquitectura recomendada

La arquitectura que usaría aquí es un **modular monolith** con clean architecture o arquitectura por capas bien separada en el backend, más frontends independientes para web y mobile. Esta combinación da velocidad de entrega, menor complejidad operativa y buena escalabilidad inicial sin caer prematuramente en microservicios. [bizzmine](https://www.bizzmine.com/es/industrias/petroleo-y-gas/)

- Backend: módulos de dominio aislados, servicios de aplicación, infraestructura separada y reglas de negocio claras.
- Frontend mobile: app híbrida con módulo offline, sincronización y almacenamiento local.
- Frontend web: app administrativa modular con rutas por dominio.
- Datos: base relacional como fuente de verdad, almacenamiento externo para archivos pesados.
- Eventos: cola o bus ligero para notificaciones, PDFs y tareas asíncronas.

No recomendaría microservicios desde el día uno, salvo que ya exista alta escala, múltiples equipos o integraciones muy exigentes. CQRS puede reservarse solo para consultas pesadas de reportes si el volumen lo justifica; para el resto, un modelo simple con servicios de aplicación y repositorios es suficiente.

## Estructura del proyecto

### Frontend mobile

- `app/`
- `features/`
- `shared/`
- `core/`
- `services/`
- `offline/`
- `sync/`
- `assets/`
- `navigation/`
- `tests/`

Responsabilidad: ejecución de inspecciones, captura offline, sincronización, firma y evidencias.

### Frontend web

- `app/`
- `modules/`
- `pages/`
- `components/`
- `shared/`
- `services/`
- `auth/`
- `reports/`
- `admin/`
- `tests/`

Responsabilidad: operación, administración, monitoreo, reportes y configuración.

### Backend API

- `src/`
- `domain/`
- `application/`
- `infrastructure/`
- `interfaces/`
- `auth/`
- `inspections/`
- `customers/`
- `assets/`
- `evidence/`
- `reports/`
- `audit/`
- `notifications/`
- `jobs/`
- `tests/`

Responsabilidad: reglas de negocio, permisos, estados, integración, persistencia y eventos.

### Base de datos

- Schemas conceptuales por dominio.
- Tablas de identidad y acceso.
- Tablas operativas de inspección.
- Tablas de documentos y evidencias.
- Tablas de auditoría y trazabilidad.
- Tablas de catálogos técnicos.

### Documentación

- Visión y alcance.
- Modelo de dominio.
- Flujos de negocio.
- Contratos API.
- Reglas de estados.
- Especificación de reportes.
- Matriz de roles y permisos.
- Manual operativo y técnico.

### Testing

- Pruebas unitarias de dominio.
- Pruebas de integración de API.
- Pruebas E2E de flujos críticos.
- Pruebas de sincronización offline.
- Pruebas de carga para reportes y archivos.
- Pruebas de seguridad y permisos.

### DevOps / despliegue

- Entornos dev, qa, staging y prod.
- CI/CD para web, mobile y backend.
- Almacenamiento seguro de evidencias.
- Backups y retención.
- Monitoreo, logs y alertas.
- Versionado de API y releases.

## Modelo de datos a alto nivel

La base de datos debe ser relacional y centrada en el estado de la inspección, la trazabilidad y el documento final. Además, los archivos grandes deben ir fuera de la base, con metadatos referenciados desde ella. [viafirma](https://www.viafirma.com/es/faqs/guia-de-uso-offline/)

- **Usuarios**: identifica personas del sistema, propósito de acceso y responsabilidad operativa; se relaciona con roles, inspecciones y auditoría; guarda nombre, documento, correo, estado, firma base y metadatos de acceso.
- **Roles**: define permisos funcionales; se relaciona con usuarios y políticas; guarda nombre, alcance y permisos.
- **Clientes**: representa empresa o persona solicitante; se relaciona con sedes, órdenes e historial; guarda razón social, contactos, identificadores y estado.
- **Sedes o inmuebles**: ubica físicamente la revisión; se relaciona con cliente e inspecciones; guarda dirección, georreferencia, referencias de acceso y observaciones.
- **Instalaciones o activos**: describe la instalación interna o unidad inspeccionable; se relaciona con sede, checklist e historial; guarda tipo, características, estado y fecha de última revisión.
- **Inspecciones**: entidad central del negocio; se relaciona con cliente, sede, técnico, checklist, evidencias, hallazgos y certificado; guarda estado, fechas, resultado, prioridad y versión.
- **Órdenes de trabajo**: organiza la ejecución operativa; se relaciona con inspecciones y asignaciones; guarda programación, responsable, ventana temporal y estado logístico.
- **Checklist**: plantilla de evaluación técnica; se relaciona con tipo de inspección y versión; guarda nombre, vigencia y configuración.
- **Ítems de checklist**: preguntas o verificaciones; se relaciona con checklist y respuestas; guarda criterio, tipo de dato, obligatoriedad y orden.
- **Respuestas de checklist**: captura la ejecución real; se relaciona con inspección e ítem; guarda valor, observación y cumplimiento.
- **Evidencias**: metadatos de fotos, PDFs, audios o archivos; se relaciona con inspección, respuesta o hallazgo; guarda tipo, ruta, hash, sello de tiempo y autor.
- **Firmas**: almacena conformidad de cliente o técnico; se relaciona con inspección o documento; guarda imagen, nombre, documento y hora.
- **Hallazgos o no conformidades**: registra problemas detectados; se relaciona con inspección, evidencia y severidad; guarda descripción, criticidad y estado de cierre.
- **Certificados o informes**: documento final emitido; se relaciona con inspección y firmantes; guarda número, versión, fecha, resultado y enlace al PDF.
- **Estados de historial**: bitácora de cambios de ciclo de vida; se relaciona con inspección y usuario; guarda estado anterior, nuevo, timestamp y motivo.
- **Auditoría**: traza acciones sensibles; se relaciona con usuario, entidad y evento; guarda acción, antes/después, IP y contexto.
- **Notificaciones**: controla avisos y recordatorios; se relaciona con usuario o cliente; guarda canal, mensaje, prioridad y entrega.
- **Parámetros y catálogos**: tipos de inspección, motivos, severidades, plantillas y reglas; centraliza configuraciones del negocio.

## Roadmap por fases

### Fase 0: descubrimiento y definición funcional

**Objetivo:** entender el proceso real de inspección y validar alcance.  
**Entregables:** mapa de procesos, roles, reglas, estados, MVP y riesgos.  
**Módulos:** negocio, operación, cumplimiento.  
**Dependencias:** stakeholders y normativa interna.  
**Prioridad:** máxima.  
**Riesgos:** alcance difuso y reglas legales incompletas.

### Fase 1: diseño del dominio y arquitectura

**Objetivo:** definir arquitectura, modelo de datos y boundaries del sistema.  
**Entregables:** dominios, contratos conceptuales, arquitectura objetivo y estrategia de sincronización.  
**Módulos:** backend, datos, mobile, web.  
**Dependencias:** fase 0.  
**Prioridad:** máxima.  
**Riesgos:** sobrediseño o inconsistencias entre web y mobile.

### Fase 2: backend base + autenticación

**Objetivo:** construir la base técnica de negocio y seguridad.  
**Entregables:** login, roles, usuarios, catálogos base, inspección mínima, auditoría inicial.  
**Módulos:** identidad, permisos, API core.  
**Dependencias:** diseño de dominio.  
**Prioridad:** alta.  
**Riesgos:** seguridad, escalabilidad temprana y control de estados.

### Fase 3: app web administrativa inicial

**Objetivo:** habilitar operación de oficina.  
**Entregables:** clientes, sedes, agenda, asignación, seguimiento y consultas.  
**Módulos:** administración, programación, monitoreo.  
**Dependencias:** backend base.  
**Prioridad:** alta.  
**Riesgos:** desalineación con el modelo real de negocio.

### Fase 4: app mobile para técnicos

**Objetivo:** permitir ejecución de campo.  
**Entregables:** agenda, inspección, checklist, evidencias y firma básica.  
**Módulos:** mobile core, offline básico, sincronización.  
**Dependencias:** API estable.  
**Prioridad:** muy alta.  
**Riesgos:** experiencia offline, conflictos de sincronización y usabilidad.

### Fase 5: evidencias, firma y certificados

**Objetivo:** cerrar el ciclo documental.  
**Entregables:** carga robusta de archivos, firmas, generación de PDF e histórico.  
**Módulos:** documentos, evidencias, certificación.  
**Dependencias:** mobile y web operativas.  
**Prioridad:** muy alta.  
**Riesgos:** peso de archivos, validez documental y versionado.

### Fase 6: reportes, dashboard y mejoras

**Objetivo:** consolidar control operativo y analítica.  
**Entregables:** KPIs, exportaciones, paneles, alertas y vencimientos.  
**Módulos:** reportes, analytics, notificaciones.  
**Dependencias:** datos confiables.  
**Prioridad:** media.  
**Riesgos:** calidad de datos y rendimiento.

### Fase 7: QA, seguridad, despliegue y documentación

**Objetivo:** preparar salida controlada a producción.  
**Entregables:** pruebas, hardening, manuales, monitoreo y plan de soporte.  
**Módulos:** calidad, devops, documentación.  
**Dependencias:** todos los módulos previos.  
**Prioridad:** crítica.  
**Riesgos:** fallos de seguridad, documentación incompleta y deuda técnica.

## MVP recomendado

El MVP debe resolver el flujo mínimo completo de una inspección de gas sin sofisticación innecesaria. La prioridad es que una operación real pueda programar, ejecutar, validar y cerrar inspecciones con trazabilidad básica. [grupovanti](https://www.grupovanti.com/rpo)

### Imprescindible

- Usuarios, roles y autenticación.
- Clientes, sedes e inspecciones.
- Agenda y asignación de técnicos.
- App mobile con checklist básico.
- Captura de evidencias y observaciones.
- Firma del cliente o responsable.
- Estado de inspección y trazabilidad básica.
- Generación de informe o certificado simple.
- Historial consultable.

### Importante pero puede esperar

- Offline avanzado con resolución de conflictos.
- Dashboards complejos.
- Catálogos extensos.
- Reglas automáticas de vencimiento.
- Integraciones externas.
- Plantillas múltiples de documentos.

### Avanzado para futuras versiones

- Analítica predictiva.
- OCR o lectura automática.
- Integraciones con terceros.
- Workflow de no conformidades.
- Múltiples tipos de inspección.
- Multiempresa o multioperador complejo.

## Backlog inicial

### Autenticación y usuarios

- Como administrador, quiero crear usuarios con roles para controlar acceso.
- Como sistema, quiero registrar intentos de ingreso y actividad.
- Como supervisor, quiero desactivar usuarios o reasignar responsabilidades.

### Clientes y sedes

- Como operador, quiero registrar clientes y sus sedes.
- Como sistema, quiero relacionar sedes con históricos de inspección.
- Como usuario, quiero buscar sedes por dirección o cliente.

### Agenda de revisiones

- Como coordinador, quiero programar inspecciones por fecha y técnico.
- Como técnico, quiero ver mi agenda diaria.
- Como supervisor, quiero reprogramar o reasignar órdenes.

### Ejecución de inspección

- Como técnico, quiero iniciar una inspección desde el móvil.
- Como técnico, quiero cambiar el estado según avance el recorrido.
- Como supervisor, quiero ver el progreso de cada inspección.

### Checklist

- Como administrador, quiero definir plantillas de checklist por tipo de inspección.
- Como técnico, quiero responder ítems obligatorios y opcionales.
- Como sistema, quiero validar consistencia de respuestas.

### Evidencias

- Como técnico, quiero adjuntar fotos y archivos a una inspección.
- Como sistema, quiero almacenar evidencias con metadatos y trazabilidad.
- Como supervisor, quiero revisar evidencias antes del cierre.

### Firma

- Como técnico, quiero capturar la firma del cliente.
- Como sistema, quiero asociar firma al documento final.
- Como administrador, quiero definir cuándo la firma es obligatoria.

### Reportes

- Como supervisor, quiero ver inspecciones por estado, técnico y cliente.
- Como administrador, quiero exportar reportes operativos.
- Como sistema, quiero generar históricos por periodo.

### Administración

- Como administrador, quiero gestionar catálogos, parámetros y plantillas.
- Como sistema, quiero auditar cambios sensibles.
- Como soporte, quiero consultar bitácoras de eventos.

## Riesgos y decisiones clave

Hay varias decisiones que conviene tomar desde el inicio porque condicionan toda la plataforma. La más importante es el equilibrio entre operación offline, trazabilidad legal y simplicidad técnica; en campo, la conectividad puede fallar, pero el dato final debe quedar íntegro y auditable. [play.google](https://play.google.com/store/apps/details?id=com.movilixa.inspecciona&hl=es)

- **Offline vs online:** conviene un offline parcial para captura de campo, con sincronización posterior y control de conflictos.
- **Evidencias pesadas:** deben ir a almacenamiento de objetos, no a la base de datos, con compresión y límites de tamaño.
- **Seguridad de datos:** cifrado en tránsito y en reposo, control de acceso por rol, auditoría y protección de documentos.
- **Control de estados:** el workflow debe ser estricto, con transiciones válidas y estados terminales bien definidos.
- **Certificados e informes:** requieren versionado, numeración única y sello temporal.
- **Permisos por rol:** deben aplicarse en API, web y mobile, no solo en interfaz.
- **Sincronización:** debe contemplar reintentos, duplicados, conflictos y cambios concurrentes.
- **Trazabilidad legal/técnica:** cada cambio importante debe dejar autor, fecha, motivo y evidencia asociada.

## Contexto adicional importante

Asumo una operación real en Colombia con necesidad de cumplir revisiones periódicas gestionadas por un organismo de inspección acreditado, y con emisión de certificado o informe al cierre de cada revisión. También asumo que el producto debe crecer desde un MVP operativo hasta una plataforma multiusuario con reportes, auditoría y capacidades offline, sin sobredimensionarse con microservicios desde el día uno. [ccconsumidores.org](https://ccconsumidores.org.co/sobre-la-revision-periodica-de-las-instalaciones-internas-de-gas/)

La recomendación práctica es construir primero el dominio de inspección y el flujo documental, luego reforzar offline, reportes y automatizaciones. Con esa secuencia, el sistema puede salir al mercado con valor real y sin sacrificar mantenibilidad, trazabilidad ni escalabilidad futura.