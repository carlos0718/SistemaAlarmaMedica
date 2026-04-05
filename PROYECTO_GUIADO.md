# Proyecto Guiado — Mini Sistema de Cine

## Objetivos

Construir un sistema pequeño de **gestión de películas y géneros cinematográficos**
aplicando los conceptos del examen. La idea es que construyas cada pieza vos mismo,
guiándote con los pasos y las preguntas orientadoras.

### Arquitectura adoptada: patrón en capas inspirado en Clean Architecture

Este proyecto sigue una **arquitectura en capas inspirada en Clean Architecture**,
adaptada al patrón de `SistemaAlarmaMedica`.
La regla general es orientar dependencias hacia capas de negocio (especialmente `Dominio`),
con decisiones pragmáticas de referencia directa desde presentación cuando aporta simplicidad.

Principios que vas a aplicar durante el desarrollo:
- **Dominio**: contiene reglas de negocio, entidades e interfaces. No depende de frameworks.
- **Infraestructura**: implementa detalles técnicos (EF Core, acceso a datos, repositorios concretos).
- **Aplicación**: conecta piezas y configura el contenedor para resolver dependencias con IoC/DI.
- **Presentación** (`MiniCineApi` y `MiniCineWeb`): recibe requests, valida entrada y delega; en este patrón también puede reutilizar contratos/DTOs de `Dominio`.

### Definiciones conceptuales clave

- **Entidad**: objeto del negocio con identidad propia (ejemplo: `Genero`, `Pelicula`).
- **DTO (Data Transfer Object)**: estructura para transportar datos entre capas o entre API y UI.
- **Repositorio**: abstracción para persistencia; desacopla reglas de negocio del motor de datos.
- **Servicio de dominio**: orquesta validaciones y reglas de negocio antes de persistir.
- **IoC (Inversión de Control)**: principio donde las clases no crean sus dependencias; ese control se delega al contenedor.
- **DI (Inyección de Dependencias)**: técnica para aplicar IoC, inyectando dependencias desde afuera (por ejemplo, por constructor).
- **ServiceResponse**: contrato estándar para devolver éxito/error sin propagar excepciones de negocio a la UI.

El sistema tendrá:
- Dos entidades: `Genero` y `Pelicula` (una Película pertenece a un Género)
- API REST que expone CRUD de ambas entidades
- App MVC que consume esa API y muestra formularios
- Sesiones de usuario y control de acceso por rol
- Validaciones con FluentValidation
- Middleware que protege rutas

---

## Estructura de proyectos a crear

```
MiniCine.sln
├── Dominio/         → Class Library       (entidades, interfaces, DTOs)
├── Infraestructura/ → Class Library       (DbContext, repositorios)
├── Aplicacion/      → Class Library       (registro de DI)
├── MiniCineApi/     → ASP.NET Core Web API (controllers REST)
└── MiniCineWeb/     → ASP.NET Core MVC    (vistas Razor, sesiones)
```

---

## Paso 0 — Crear la solución y los proyectos

Abrí una terminal en la carpeta donde querés trabajar y ejecutá estos comandos
**en orden**. Cada línea hace una cosa concreta.

### 0.1 — Crear la solución

```bash
dotnet new sln -n MiniCine
```

Esto crea el archivo `MiniCine.sln` que agrupa todos los proyectos.

### 0.2 — Crear cada proyecto

```bash
# Capas internas: Class Library (solo código C#, sin servidor)
dotnet new classlib -n Dominio
dotnet new classlib -n Infraestructura
dotnet new classlib -n Aplicacion

# Capa de API: Web API (tiene Program.cs y levanta un servidor HTTP)
dotnet new webapi -n MiniCineApi

# Capa MVC: aplicación web con vistas Razor
dotnet new mvc -n MiniCineWeb
```

> **¿Por qué Class Library para Dominio, Infraestructura y Aplicacion?**
> Porque son capas de lógica pura que no tienen que levantar ningún servidor.
> Solo exponen clases e interfaces que otros proyectos consumen.
> `webapi` y `mvc` sí levantan servidores porque tienen su propio `Program.cs`.

### 0.3 — Agregar los proyectos a la solución

```bash
dotnet sln add Dominio
dotnet sln add Infraestructura
dotnet sln add Aplicacion
dotnet sln add MiniCineApi
dotnet sln add MiniCineWeb
```

### 0.4 — Configurar las referencias entre proyectos

Cada capa solo puede ver las capas que están **por debajo** de ella:
Gráfico básico de capas (alineado al patrón de `SistemaAlarmaMedica`):

```text
MiniCineWeb  ───────────────► Aplicacion
MiniCineWeb  ───────────────► Dominio

MiniCineApi  ───────────────► Aplicacion
MiniCineApi  ───────────────► Infraestructura
MiniCineApi  ───────────────► Dominio

Aplicacion   ───────────────► Infraestructura
Infraestructura ────────────► Dominio

Flecha = referencia de proyecto (dependencia de código)
```

```bash
# Infraestructura necesita las entidades e interfaces de Dominio
dotnet add Infraestructura reference Dominio

# Aplicacion usa Infraestructura (y por transitividad accede a Dominio)
dotnet add Aplicacion reference Infraestructura

# La API referencia Aplicacion, Infraestructura y Dominio
dotnet add MiniCineApi reference Aplicacion
dotnet add MiniCineApi reference Infraestructura
dotnet add MiniCineApi reference Dominio

# La app MVC referencia Aplicacion y Dominio
dotnet add MiniCineWeb reference Aplicacion
dotnet add MiniCineWeb reference Dominio
```

> **Nota importante:** este esquema refleja el patrón usado en `SistemaAlarmaMedica`.
> Es una variante pragmática: la capa de presentación referencia también `Dominio`
> para reutilizar contratos/DTOs, aunque no sea la forma más estricta de Clean Architecture.

### 0.5 — Instalar paquetes NuGet necesarios

```bash
# Infraestructura: EF Core + SQL Server + migraciones + configuración
dotnet add Infraestructura package Microsoft.EntityFrameworkCore
dotnet add Infraestructura package Microsoft.EntityFrameworkCore.SqlServer
dotnet add Infraestructura package Microsoft.EntityFrameworkCore.Design
dotnet add Infraestructura package Microsoft.EntityFrameworkCore.Tools
dotnet add Infraestructura package Microsoft.Extensions.Configuration
dotnet add Infraestructura package Microsoft.Extensions.Configuration.Json
dotnet add Infraestructura package Microsoft.Extensions.Options.ConfigurationExtensions

# Aplicacion: DI + HttpClient + soporte de tooling
dotnet add Aplicacion package Microsoft.Extensions.DependencyInjection.Abstractions
dotnet add Aplicacion package Microsoft.Extensions.Http
dotnet add Aplicacion package Microsoft.EntityFrameworkCore.Design

# Dominio: mapeos y utilidades de serialización
dotnet add Dominio package AutoMapper
dotnet add Dominio package Microsoft.EntityFrameworkCore
dotnet add Dominio package Newtonsoft.Json

# MiniCineWeb: validaciones y autenticación
dotnet add MiniCineWeb package FluentValidation.AspNetCore
dotnet add MiniCineWeb package Microsoft.AspNetCore.Authentication.Google

# MiniCineApi: Swagger + tooling de diseño EF
dotnet add MiniCineApi package Swashbuckle.AspNetCore
dotnet add MiniCineApi package Microsoft.EntityFrameworkCore.Design

# Opcional en Web (si usás scaffolding o tooling EF)
dotnet add MiniCineWeb package Microsoft.EntityFrameworkCore.Design
dotnet add MiniCineWeb package Microsoft.VisualStudio.Web.CodeGeneration.Design
```

### 0.6 — Verificar que todo compila

```bash
dotnet build MiniCine.sln
```

Si compila sin errores, la estructura base está lista y podés empezar el Paso 1.

---

## Paso 1 — Entidades del Dominio

**Archivos a crear en `Dominio/Entidades/`**

Pensá en los campos que necesita cada entidad:

- `Genero`: identificador, nombre (Acción, Drama, Comedia...), descripción
- `Pelicula`: identificador, título, director, año de estreno, duración en minutos,
  y la FK hacia `Genero` (tanto el campo entero `GeneroId` como la propiedad de
  navegación `Genero`)
- `TipoUsuario`: identificador y nombre (representa el rol: `ADMINISTRADOR` o `EMPLEADO`).
  Es una entidad de lookup — se persiste como tabla en la BD.
- `Usuario`: identificador, nombre, email, contraseña, y la FK hacia `TipoUsuario`
  (tanto el campo entero `TipoUsuarioId` como la propiedad de navegación `TipoUsuario`)

> **Pregunta orientadora:** ¿Por qué el campo `GeneroId` va en `Pelicula` y no al revés?
> ¿Qué pasaría en la base de datos si lo pusieras al revés?

---

## Paso 2 — Interfaces genéricas (Bloque 1 y 2)

**Archivo a crear en `Dominio/Core/Genericos/IRepository.cs`**

Definí una interfaz genérica con restricción de tipo para que solo acepte
clases. Debe declarar al menos cinco operaciones:
obtener por id, obtener todos, agregar, actualizar y eliminar.
Todas deben ser asíncronas y retornar `Task` o `Task<T>`.

> **Pregunta orientadora:** ¿Qué hace la restricción `where TEntity : class`?
> ¿Qué pasaría si la quitaras e intentaras usar un `int` como tipo genérico?

---

## Paso 3 — Interfaces específicas de repositorio

**Archivos a crear en `Dominio/Servicios/Generos/` y `Dominio/Servicios/Peliculas/`**

- `IGeneroRepository`: extiende la interfaz genérica. No necesita métodos extra.
- `IPeliculaRepository`: extiende la interfaz genérica. Agregale un método
  extra `ObtenerPorGeneroAsync(int generoId)` que retorne una lista de películas
  de ese género.

> **Pregunta orientadora:** ¿Por qué `IPeliculaRepository` extiende la interfaz genérica
> en lugar de declararla de cero con todos los métodos?

---

## Paso 4 — ServiceResponse

**Archivo a crear en `Dominio/Shared/ServiceResponse.cs`**

Creá las dos versiones: `ServiceResponse<T>` con datos genéricos, y `ServiceResponse`
que hereda de ella pero es no genérica (para operaciones que no retornan datos).

La clase debe tener:
- Una lista de errores
- Propiedades calculadas `IsSuccess` e `IsFailure` que deriven de si hay errores
- Métodos para agregar errores (uno que acepte `Exception`, otro que acepte `string`)
- Métodos estáticos de fábrica: `Success()`, `Success<T>(data)` y `Failure(errores)`

> **Pregunta orientadora:** ¿Por qué se usan métodos de fábrica estáticos en vez de
> simplemente hacer `new ServiceResponse()`? ¿Qué ventaja da en legibilidad?

---

## Paso 5 — DTOs

**Archivos a crear en `Dominio/Application/DTOs/`**

Por cada entidad creá su DTO. Las diferencias respecto a la entidad:
- Los ids son `int?` (nullable), porque al crear un objeto nuevo no hay id todavía
- El DTO de `Pelicula` tiene tanto el `GeneroId` como un `GeneroDto?` anidado
  (para poder mostrar el nombre del género en las vistas sin una segunda consulta)

Creá también un DTO para filtros: `FiltroPeliculaDto` con un campo `string? Titulo`
y otro `int? GeneroId`, que se usarán como query string en la API.

> **Pregunta orientadora:** ¿Por qué el DTO tiene `GeneroDto?` anidado en vez de
> solo el `GeneroId`? ¿En qué operaciones usarías uno y en cuál el otro?

---

## Paso 6 — Interfaces de servicio

**Archivos a crear en `Dominio/Servicios/Generos/` y `Dominio/Servicios/Peliculas/`**

Cada interfaz de servicio debe declarar las operaciones disponibles para esa entidad:
- `IGeneroService`: obtener todos (con filtro de nombre opcional), obtener por id,
  agregar, modificar, eliminar
- `IPeliculaService`: lo mismo más un método para obtener películas de un género

Todas las operaciones de escritura retornan `ServiceResponse`.
Las de lectura retornan `ServiceResponse<T>` con los datos.

---

## Paso 7 — AutoMapper

**Archivo a crear en `Dominio/Application/Mappings/AutoMapperProfile.cs`**

Configurá los mapeos bidireccionales entre cada entidad y su DTO.
Recordá usar `.ReverseMap()` para no tener que declararlos dos veces.

> **Pregunta orientadora:** `Pelicula` tiene una propiedad `Genero` (entidad)
> y `PeliculaDto` tiene una propiedad `Genero` (DTO). ¿AutoMapper las mapea
> automáticamente por tener el mismo nombre, o necesitás configuración extra?

---

## Paso 8 — DbContext (Infraestructura)

**Archivo a crear en `Infraestructura/ContextoBD/AppDbContexto.cs`**

El contexto debe:
1. Exponer un `DbSet<T>` por cada entidad
2. En `OnConfiguring`: leer la cadena de conexión primero desde variable de entorno
   y si no existe, leerla desde `appsettings.json`
3. En `OnModelCreating`: configurar con Fluent API:
   - La relación `Pelicula` → `Genero` (muchos a uno)
   - Decidí si al eliminar un Género que tiene Películas usás `Restrict` o `Cascade`,
     y justificá tu elección con un comentario en el código

> **Pregunta orientadora:** ¿Cuántas instancias del DbContext existen durante una
> request HTTP si lo registraste como `AddScoped`? ¿Y si usaras `AddTransient`?

---

## Paso 9 — Repositorios concretos (Infraestructura)

**Archivos a crear en `Infraestructura/Repositorios/`**

Primero creá el `Repository<TEntity>` genérico que implemente `IRepository<TEntity>`.
Inyectá el contexto por constructor y usá `_context.Set<TEntity>()` para acceder
a cada tabla sin referencias directas a los `DbSet`.

Luego creá `GeneroRepository` y `PeliculaRepository`:
- Ambos heredan del genérico y también implementan su interfaz específica
- `PeliculaRepository` implementa `ObtenerPorGeneroAsync`: hacé un `Where`
  filtrando por `GeneroId` e incluí la navegación a `Genero` con `Include`

> **Pregunta orientadora:** ¿Por qué el repositorio concreto llama a
> `base(dbContext)` en su constructor? ¿Qué pasaría si no lo hicieras?

---

## Paso 10 — Implementaciones de Servicio (Dominio)

**Archivos a crear en `Dominio/Servicios/Generos/GeneroService.cs`** y similar para Pelicula

Cada servicio recibe por constructor su repositorio (la interfaz) y el `IMapper`.

Lógica a implementar en `GeneroService`:
- `AgregarAsync`: verificá que no exista otro género con el mismo nombre antes
  de guardar. Si existe, agregá el error al `ServiceResponse` con un mensaje claro.
  Usá try/catch y nunca relances la excepción hacia afuera.
- `EliminarAsync`: antes de eliminar, verificá si el género tiene películas
  asociadas. Si las tiene, rechazá la operación con un mensaje descriptivo.
- Para las demás operaciones: mapeá con AutoMapper y delegá al repositorio.

Lógica a implementar en `PeliculaService`:
- `AgregarAsync`: verificá que no exista otra película con el mismo título y
  el mismo año de estreno (dos films distintos pueden tener el mismo título
  si son de años distintos).

> **Pregunta orientadora:** ¿Por qué en el servicio se inyecta `IGeneroRepository`
> y no `GeneroRepository` directamente?

---

## Paso 11 — Registro de DI

**Archivo a crear en `Aplicacion/AddIoC.cs`**

Creá el método de extensión `AddInversionOfControl` sobre `IServiceCollection`.
Registrá en el orden correcto:
1. AutoMapper
2. DbContext como `AddScoped`
3. El repositorio genérico (`typeof(IRepository<>)` → `typeof(Repository<>)`) como `AddScoped`
4. Repositorios específicos como `AddTransient`
5. Servicios de dominio como `AddTransient`

> **Pregunta orientadora:** Si registrás el DbContext como `AddTransient` en vez de
> `AddScoped`, ¿qué problema concreto podría ocurrir dentro de una request que hace
> dos operaciones seguidas contra la base de datos?

---

## Paso 12 — API Controllers

**Archivos a crear en `MiniCineApi/Controllers/`**

Creá `GeneroController` y `PeliculaController`. Para `PeliculaController`:

| Método | Ruta | Origen del parámetro |
|--------|------|----------------------|
| GET | `Pelicula/obtenerTodos` | `[FromQuery] FiltroPeliculaDto` |
| GET | `Pelicula/obtenerPorId/{id}` | `[FromRoute] int id` |
| GET | `Pelicula/obtenerPorGenero/{generoId}` | `[FromRoute] int generoId` |
| POST | `Pelicula/agregar` | `[FromBody] PeliculaDto` |
| PUT | `Pelicula/modificar` | `[FromBody] PeliculaDto` |
| DELETE | `Pelicula/eliminar/{id}` | `[FromRoute] int id` |

Cada acción solo llama al servicio y retorna `Ok(resultado)`.
No hay lógica de negocio en los controladores.

> **Pregunta orientadora:** ¿Por qué todos los controladores de la API retornan
> `Ok()` aunque la operación haya fallado con `IsFailure`? ¿Quién decide si fue
> un error de negocio y quién lo muestra al usuario?

---

## Paso 13 — Middleware de sesión

**Archivo a crear en `MiniCineWeb/Middleware/SessionValidationMiddleware.cs`**

El middleware debe:
1. Recibir el `RequestDelegate _next` en el constructor
2. En `InvokeAsync(HttpContext context)`:
   - Definir un array de rutas públicas: `/Login`, `/Home/Error`
   - Si la ruta actual NO empieza por ninguna ruta pública, verificar que haya
     un `Sesion_UsuarioId` en la sesión
   - Si no hay sesión, interrumpir el pipeline redirigiendo a `/Login`
   - Si hay sesión, llamar a `await _next(context)`
3. Creá también la clase de extensión con el método `UseSessionValidation()`

> **Pregunta orientadora:** ¿Qué pasa con una request a `/Login/Index` si el array
> de rutas públicas solo contiene `/Login`? ¿El `StartsWith` lo cubre o necesitás
> agregar `/Login/Index` por separado?

---

## Paso 14 — Filtro de autorización por rol

**Archivo a crear en `MiniCineWeb/Attributes/RoleAuthorizationAttribute.cs`**

El filtro debe implementar `IAuthorizationFilter` y recibir por constructor
un array `params` de roles permitidos.

En `OnAuthorization`:
- Leer `Sesion_UsuarioId` y `Sesion_UsuarioTipo` de la sesión
- Si no existe la sesión → redirigir al login
- Si el tipo de usuario no está en los roles permitidos → redirigir a `AccesoDenegado`

Aplicalo de forma que solo los `ADMINISTRADOR` puedan agregar o eliminar géneros,
pero los `EMPLEADO` sí puedan ver el listado de películas.

> **Pregunta orientadora:** ¿Cuál es la diferencia entre el Middleware de sesión
> del paso anterior y este filtro? ¿Podrías reemplazar uno con el otro?

---

## Paso 15 — Reutilizar DTOs de Dominio en el proyecto MVC

Como en este patrón `MiniCineWeb` referencia `Dominio`, podés reutilizar
directamente los DTOs definidos en `Dominio/Application/DTOs/` sin duplicarlos.

> **Pregunta orientadora:** ¿Qué ventaja práctica te da reutilizar estos DTOs
> y qué costo de acoplamiento introduce entre `MiniCineWeb` y `Dominio`?

---

## Paso 16 — Servicios Web (clientes HTTP)

**Archivo a crear en `MiniCineWeb/Services/PeliculaServiceWeb.cs`**

Este servicio recibe el `HttpClientService` por constructor y tiene métodos que:
- Construyen la URL del endpoint de la API correspondiente
- Hacen GET, POST, PUT o DELETE
- Deserializan la respuesta al tipo correcto de DTO

No hay lógica de negocio aquí. Solo traduce entre llamadas HTTP y objetos C#.

---

## Paso 17 — ViewModels y Controlador MVC

**Archivos a crear en `MiniCineWeb/ViewModels/` y `MiniCineWeb/Controllers/`**

Creá el ViewModel `GestionarPeliculaViewModel` con:
- El `PeliculaDto` a gestionar
- La lista de `GeneroDto` para el dropdown del formulario
- El `TipoOperacion` (enum: AGREGAR / MODIFICAR)
- El `ServiceResponse` para mostrar errores del servidor en la vista

En el controlador MVC `PeliculaController`:
- El GET de `GestionarPelicula` carga el ViewModel: si hay id carga la película
  existente, si no crea un `PeliculaDto` vacío. Siempre carga la lista de géneros.
  Retorna `View(model)`.
- El POST valida con FluentValidation, llama al servicio web y:
  - Si `IsSuccess` → `RedirectToAction("Index")`
  - Si `IsFailure` → recarga la lista de géneros, asigna el `ServiceResponse`
    al ViewModel y retorna `View(model)` con los errores visibles

---

## Paso 18 — FluentValidation

**Archivo a crear en `MiniCineWeb/Tools/Validators/PeliculaValidator.cs`**

Creá un validador para `GestionarPeliculaViewModel` con estas reglas:
- `Titulo`: requerido, máximo 150 caracteres
- `Director`: requerido, solo letras y espacios
- `AnioEstreno`: requerido, mayor o igual a 1888 (año de la primera película
  de la historia) y menor o igual al año actual
- `DuracionMinutos`: requerido, mayor a 0
- `GeneroId`: no puede ser nulo

Usá los métodos de FluentValidation: `NotNull()`, `NotEmpty()`, `MaximumLength()`,
`Matches()`, `GreaterThanOrEqualTo()`, `LessThanOrEqualTo()`, cada uno con
su propio `.WithMessage()`.

---

## Paso 19 — Flujo completo a mano

Sin escribir código, describí en tu cuaderno el recorrido completo de esta operación:

**"El administrador hace POST para agregar una nueva película con género = 2 (Drama)"**

Completá cada punto con los nombres concretos de tus clases:

1. ¿A qué acción del controlador MVC llega el POST?
2. ¿Qué validador se ejecuta primero?
3. ¿A qué método del servicio web llama?
4. ¿A qué endpoint HTTP de la API llega?
5. ¿Qué controlador de la API recibe la request?
6. ¿Qué método del servicio de dominio se llama?
7. ¿Qué validación de negocio hace el servicio? (título + año duplicado)
8. ¿Qué hace AutoMapper en este paso?
9. ¿Qué repositorio y método persisten el dato?
10. ¿Qué SQL genera EF Core internamente?
11. ¿Qué retorna el servicio de dominio?
12. ¿Qué retorna el controlador de la API?
13. ¿Qué hace el controlador MVC con esa respuesta?

---

## Checklist de conceptos cubiertos

Marcá cada ítem cuando sientas que podés explicarlo en voz alta:

- [ ] Interfaz genérica con restricción de tipo (`where TEntity : class`)
- [ ] Por qué cada servicio/repositorio tiene su propia interfaz
- [ ] Diferencia entre `AddTransient`, `AddScoped` y `AddSingleton`
- [ ] Por qué `ServiceResponse` no lanza excepciones hacia afuera
- [ ] Cómo `Include` / `ThenInclude` cargan relaciones en EF Core
- [ ] Diferencia entre `[FromQuery]`, `[FromBody]` y `[FromRoute]`
- [ ] Diferencia entre un controlador MVC y uno de API
- [ ] Cuándo conviene reutilizar DTOs de `Dominio` desde `MiniCineWeb` y qué acoplamiento introduce
- [ ] Qué pasa si no llamás `await _next(context)` en el middleware
- [ ] Para qué sirve `RoleAuthorizationAttribute` vs el middleware de sesión
- [ ] Cómo AutoMapper convierte entre entidad y DTO en ambas direcciones
- [ ] Por qué existe el patrón Repository si EF Core ya es una abstracción
