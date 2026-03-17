# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build entire solution
dotnet build SistemaAlarmaMedica.sln

# Run the REST API (port 7131 HTTPS / 5000 HTTP)
dotnet run --project PresentacionApi

# Run the MVC web app (calls the API via HttpClientService)
dotnet run --project Presentacion

# Apply database schema (no EF migrations — run SQL scripts manually)
# Scripts are in Infraestructura/Scripts/DDL.sql and patch scripts
```

There are no automated tests in this project.

## Architecture

This is a **Clean Architecture** solution targeting .NET with SQL Server. The solution has 6 projects:

### Layer order (inner to outer)

1. **Dominio** — Domain layer (no external dependencies)
   - `Entidades/` — EF entity classes (Paciente, Medico, Turno, OrdenMedica, LineaOrdenMedica, Farmaco, Especialidad, Usuario, etc.)
   - `Servicios/` — Domain service interfaces and implementations (one folder per aggregate: Farmacos, Medicos, OrdenesMedicas, Pacientes, Turnos, Usuarios, Utils)
   - `Core/Genericos/` — Generic `IRepository<T>` interface
   - `Application/DTOs/` — DTOs for all entities
   - `Application/Mappings/AutoMapperProfile.cs` — AutoMapper configuration

2. **Infraestructura** — Data access layer
   - `ContextoBD/AplicacionBDContexto.cs` — EF Core DbContext; reads connection string from env var `ConnectionStrings__LocalDbConnection` first, then falls back to `appsettings.json`
   - `Repositorios/` — Specific repositories (MedicoRepository, PacienteRepository, etc.) plus generic `Repository<T>`
   - `Scripts/` — Raw SQL DDL and patch scripts (no EF migrations)

3. **Aplicacion** — Composition root / IoC registration
   - `AddIoC.cs` — Single static extension method `AddInversionOfControl()` that registers all domain services, repositories, AutoMapper, and the external CIMA HTTP client

4. **PresentacionApi** — REST API (ASP.NET Core Web API)
   - Calls `builder.Services.AddInversionOfControl()` from Aplicacion
   - Swagger/OpenAPI enabled always (including production)
   - Controllers mirror domain aggregates: Farmaco, Medico, OrdenMedica, Paciente, Turno, Usuario

5. **Presentacion** — MVC web app (Razor Views)
   - Does **not** use domain services directly — communicates exclusively with PresentacionApi through `HttpClientService`
   - `Services/` — Web-layer service wrappers (e.g., `FarmacoServiceWeb`, `MedicoServiceWeb`) that call the API endpoints
   - Handles authentication: cookie-based sessions + Google OAuth 2.0
   - `Middleware/SessionValidation` — validates session on every request
   - API base URL configured in `appsettings.json` under `ApiSettings:BaseUrl`

6. **DefaultDI** — Empty/placeholder project (unused)

### Key design decisions

- **No EF migrations**: Schema is managed via raw SQL scripts in `Infraestructura/Scripts/`. Run `DDL.sql` to create the database, then apply patch scripts as needed.
- **Two-app architecture**: The web app (Presentacion) is a thin client that talks to PresentacionApi over HTTP. All business logic lives in the API + domain layer.
- **External API integration**: `CimaHttpClient` integrates with the Spanish AEMPS medication database (`https://cima.aemps.es/cima/rest/`) for drug lookups.
- **Connection string resolution**: `AplicacionBDContexto` reads from env var `ConnectionStrings__LocalDbConnection` first (for Docker/Azure), then from `appsettings.json` in the current directory.

## Configuration

- **Database**: SQL Server. Local default: `Server=charly\\SQLEXPRESS;Database=SistemaAlarmaMedicaBD`
- **Google OAuth**: Credentials go in `Presentacion/appsettings.json` under `Authentication:Google:ClientId` and `Authentication:Google:ClientSecret`. See `GOOGLE_OAUTH_SETUP.md` for setup steps.
- **API URL** (for Presentacion): Set `ApiSettings:BaseUrl` in `Presentacion/appsettings.json`.
- **Production**: Use environment variables to override connection string and Google credentials instead of modifying `appsettings.json`.

## Patterns to follow

### Service return type
All domain service write operations return `ServiceResponse` or `ServiceResponse<T>` (in `Dominio/Shared/`). Use the factory methods: `ServiceResponse.Success()`, `ServiceResponse.Success<T>(data)`, `ServiceResponse.Failure(errors)`. Check `response.IsSuccess` / `response.IsFailure`. Never throw exceptions for business errors — add them via `response.AddError(...)`.

### Role-based authorization (Presentacion)
Use `[RoleAuthorization(TipoUsuarioDto.MEDICO, TipoUsuarioDto.ADMINISTRADOR)]` on MVC controllers/actions. The filter reads `Sesion_UsuarioId` and `Sesion_UsuarioTipo` from the session. The three roles are `ADMINISTRADOR = 1`, `MEDICO = 2`, `PACIENTE = 3`.

### Duplicate DTOs
`Presentacion` has its own copy of all DTOs in `Presentacion/Core/DTOs/` that mirror `Dominio/Application/DTOs/`. When adding a new field to a domain DTO, update the matching Presentacion DTO too. They are not shared because `Presentacion` does not reference `Dominio` directly.
