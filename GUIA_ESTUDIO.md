# Guía de Estudio — Examen Final

## Tecnología: C# / .NET con ASP.NET Core

Este proyecto usa **Clean Architecture** con 6 capas. Todo lo que estudies está basado en código real del proyecto.

---

## Bloque 1: Fundamentos de C# que usa este proyecto

Antes de entender los patrones, necesitas dominar esto:

### 1.1 Interfaces y clases abstractas

El proyecto las usa masivamente. Cada servicio y repositorio tiene una interfaz.

```csharp
// Dominio/Servicios/Medicos/IMedicoService.cs
public interface IMedicoService
{
    Task<MedicoDto> ObtenerPorIdAsync(int id);
    Task<ServiceResponse> AgregarAsync(MedicoDto entity);
}

// Dominio/Servicios/Medicos/MedicoService.cs
public class MedicoService : IMedicoService  // implementa la interfaz
{
    public async Task<MedicoDto> ObtenerPorIdAsync(int id) { ... }
}
```

**Lo que debes saber:** ¿Por qué usar interfaces? Para desacoplar: si mañana cambias la implementación, el resto del código no cambia.

### 1.2 Genéricos (`<T>`)

El `IRepository<T>` y `Repository<T>` son genéricos.

```csharp
// Un solo Repository sirve para CUALQUIER entidad
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    public async Task<TEntity> GetByIdAsync(int id) { ... }
    public async Task AddAsync(TEntity entity) { ... }
}
```

**Lo que debes saber:** qué es `where TEntity : class`, cómo se instancia un genérico.

### 1.3 async/await y Tasks

Todo el proyecto es asíncrono.

```csharp
public async Task<MedicoDto> ObtenerPorIdAsync(int id)
{
    var medico = await _medicoRepository.GetByIdAsync(id); // espera sin bloquear
    return _mapper.Map<MedicoDto>(medico);
}
```

**Lo que debes saber:** diferencia entre `Task`, `Task<T>`, `async void`. Por qué se usa async en aplicaciones web.

### 1.4 LINQ

Se usa para filtrar listas en los servicios.

```csharp
// Dominio/Servicios/OrdenesMedicas/OrdenMedicaService.cs
ordenesDb = ordenesDb
    .Where(o => o.PacienteId == pacienteId.Value)
    .ToList();

// Verificar duplicado en MedicoService
var validarMatricula = (await _medicoRepository.GetAllAsync())
    .Any(m => m.Matricula == entity.Matricula);
```

**Métodos LINQ a estudiar:** `Where`, `Any`, `FirstOrDefault`, `Select`, `ToList`, `Contains`.

---

## Bloque 2: Patrones de Diseño del Proyecto

### 2.1 Repository Pattern (el más importante del proyecto)

**Archivos clave:**
- `Dominio/Core/Genericos/IRepository.cs` — interfaz genérica
- `Infraestructura/Genericos/Repository.cs` — implementación genérica
- `Dominio/Servicios/Medicos/IMedicoRepository.cs` — interfaz específica
- `Infraestructura/Repositorios/MedicoRepository.cs` — repositorio específico

**El flujo:**
```
IRepository<T>  (interfaz genérica)
    ↑ extiende
IMedicoRepository  (interfaz específica con métodos propios)
    ↑ implementa
MedicoRepository : Repository<Medico>, IMedicoRepository
```

**Interfaz genérica:**
```csharp
// Dominio/Core/Genericos/IRepository.cs
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(int id);
    Task<List<TEntity>> GetAllAsync();
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
}
```

**Implementación genérica:**
```csharp
// Infraestructura/Genericos/Repository.cs
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly IAplicacionBDContexto _context;

    public Repository(IAplicacionBDContexto context)
    {
        _context = context;
    }

    public async Task<TEntity> GetByIdAsync(int id)
        => await _context.Set<TEntity>().FindAsync(id);

    public async Task<List<TEntity>> GetAllAsync()
        => await _context.Set<TEntity>().ToListAsync();

    public async Task AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}
```

**Repositorio específico (extiende el genérico):**
```csharp
// Infraestructura/Repositorios/MedicoRepository.cs
public class MedicoRepository : Repository<Medico>, IMedicoRepository
{
    private readonly IAplicacionBDContexto _dbContext;

    public MedicoRepository(IAplicacionBDContexto dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    // Método propio de este repositorio
    public async Task<List<Especialidad>> ObtenerEspecialidadesAsync()
        => await _dbContext.Set<Especialidad>().ToListAsync();
}
```

**Lo que debes saber:** ¿Por qué existe el Repository? Para aislar el acceso a datos. Si cambias de SQL Server a otra BD, solo cambias los repositorios, no el dominio.

### 2.2 Dependency Injection (DI)

**Archivo clave:** `Aplicacion/AddIoC.cs`

Todo se registra en un único lugar:

```csharp
// Aplicacion/AddIoC.cs
public static class AddIoC
{
    public static IServiceCollection AddInversionOfControl(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddScoped<IAplicacionBDContexto, AplicacionBDContexto>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddTransient<IMedicoService, MedicoService>();
        services.AddTransient<IMedicoRepository, MedicoRepository>();
        services.AddTransient<IPacienteService, PacienteService>();
        services.AddTransient<IPacienteRepository, PacienteRepository>();
        // ...resto de servicios y repositorios

        services.AddHttpClient<ICimaHttpClient, CimaHttpClient>(client =>
        {
            client.BaseAddress = new Uri("https://cima.aemps.es/cima/rest/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
```

Luego se inyecta automáticamente en constructores:
```csharp
public class MedicoController : ControllerBase
{
    private readonly IMedicoService _medicoService;

    public MedicoController(IMedicoService medicoService) // .NET lo resuelve solo
    {
        _medicoService = medicoService;
    }
}
```

**Lo que debes saber:**

| Ciclo de vida | Descripción | Uso en este proyecto |
|---|---|---|
| `AddTransient` | Nueva instancia cada vez que se pide | Servicios y repositorios |
| `AddScoped` | Una instancia por request HTTP | DbContext |
| `AddSingleton` | Una instancia para toda la vida del app | No se usa aquí |

### 2.3 DTO Pattern (Data Transfer Objects)

**Archivos clave:** `Dominio/Application/DTOs/MedicoDto.cs`

Los DTOs viajan entre capas. Las **Entidades** son para EF/base de datos. Los **DTOs** son para mostrar/recibir datos.

```
Entidad (Medico)   → solo en la capa de BD/Dominio
DTO (MedicoDto)    → lo que ve la API y la presentación
```

```csharp
// Dominio/Application/DTOs/MedicoDto.cs
public class MedicoDto
{
    public int? MedicoId { get; set; }
    public string? Apellido { get; set; }
    public string? Nombre { get; set; }
    public string? Matricula { get; set; }
    public int? EspecialidadId { get; set; }
    public EspecialidadDto? Especialidad { get; set; }
}
```

**Atención:** `Presentacion` tiene sus propios DTOs en `Presentacion/Core/DTOs/` porque no referencia directamente a `Dominio`. Son una copia idéntica pero en otro proyecto.

### 2.4 ServiceResponse Pattern

**Archivo clave:** `Dominio/Shared/ServiceResponse.cs`

En lugar de lanzar excepciones para errores de negocio, se devuelve un objeto que indica si fue exitoso o no:

```csharp
// Dominio/Shared/ServiceResponse.cs
public class ServiceResponse<T>
{
    public List<ServiceError> Errors { get; set; } = new();
    public T Data { get; set; }
    public bool IsSuccess => !IsFailure;
    public bool IsFailure => Errors.Any();

    public void AddError(Exception ex) { Errors.Add(new ServiceError(ex)); }
    public void AddError(string message) { Errors.Add(new ServiceError(message)); }
    public string GetErrorsAsString() => string.Join("; ", Errors.Select(e => e.ErrorMessage));
}

public class ServiceResponse : ServiceResponse<object>
{
    public static ServiceResponse Success() => new ServiceResponse();
    public static ServiceResponse<T> Success<T>(T data) => new ServiceResponse<T> { Data = data };
    public static ServiceResponse Failure(List<ServiceError> errors) => new ServiceResponse { Errors = errors };
}
```

**Uso en un servicio:**
```csharp
public async Task<ServiceResponse> AgregarAsync(MedicoDto entity)
{
    var response = new ServiceResponse();
    try
    {
        var duplicado = (await _medicoRepository.GetAllAsync())
            .Any(m => m.Matricula == entity.Matricula);

        if (duplicado)
            throw new InvalidOperationException($"La matricula {entity.Matricula} ya existe.");

        var medico = _mapper.Map<Medico>(entity);
        await _medicoRepository.AddAsync(medico);
    }
    catch (Exception ex)
    {
        response.AddError(ex); // nunca explota hacia arriba
    }
    return response;
}
```

**Uso en el controlador MVC:**
```csharp
var response = await _medicoServiceWeb.Agregar(model.Medico);

if (response.IsSuccess)
    return RedirectToAction("Index");
else
    model.RespuestaServidor = response; // muestra errores en la vista
    return View(model);
```

---

## Bloque 3: Entity Framework Core

**Archivo clave:** `Infraestructura/ContextoBD/AplicacionBDContexto.cs`

### 3.1 DbContext

```csharp
public class AplicacionBDContexto : DbContext
{
    public DbSet<Medico> Medicos { get; set; }       // representa la tabla Medicos
    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<OrdenMedica> OrdenMedicas { get; set; }
    // ...
}
```

### 3.2 Configuración de cadena de conexión

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    // Prioridad 1: variable de entorno (Docker/Azure)
    var connection = Environment.GetEnvironmentVariable("ConnectionStrings__LocalDbConnection");

    // Prioridad 2: appsettings.json (local)
    if (string.IsNullOrEmpty(connection))
    {
        connection = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build()
            .GetSection("ConnectionStrings")["LocalDbConnection"];
    }

    optionsBuilder.UseSqlServer(connection);
}
```

### 3.3 Configuración de relaciones (Fluent API)

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Relación 1-N: Medico tiene 1 Especialidad
    modelBuilder.Entity<Medico>()
        .HasOne(m => m.Especialidad)
        .WithMany()
        .HasForeignKey(m => m.EspecialidadId)
        .IsRequired();

    // Cascade delete: eliminar OrdenMedica elimina sus líneas
    modelBuilder.Entity<OrdenMedica>()
        .HasMany(o => o.LineaOrdenMedica)
        .WithOne(l => l.OrdenMedica)
        .HasForeignKey(l => l.OrdenMedicaId)
        .OnDelete(DeleteBehavior.Cascade);

    // Restrict: no se puede eliminar un Medico si tiene órdenes
    modelBuilder.Entity<OrdenMedica>()
        .HasOne(o => o.Medico)
        .WithMany()
        .HasForeignKey(o => o.MedicoId)
        .OnDelete(DeleteBehavior.Restrict);
}
```

### 3.4 Include / ThenInclude (carga de relaciones)

Sin `Include`, las propiedades de navegación llegan `null`:

```csharp
// Cargar orden médica con todas sus relaciones anidadas
var orden = await _context.OrdenMedicas
    .Include(o => o.Paciente)
    .Include(o => o.Medico)
        .ThenInclude(m => m.Especialidad)  // relación anidada: Medico.Especialidad
    .Include(o => o.LineaOrdenMedica)
    .FirstOrDefaultAsync(o => o.OrdenMedicaId == id);
```

---

## Bloque 4: ASP.NET Core Web API

**Archivos clave:** `PresentacionApi/Controllers/MedicoController.cs`

### 4.1 Verbos HTTP y sus atributos

```csharp
[Route("Medico")]
[ApiController]
public class MedicoController : ControllerBase
{
    [HttpGet("obtenerPorId/{id}")]         // GET /Medico/obtenerPorId/5
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var result = await _medicoService.ObtenerPorIdAsync(id);
        return Ok(result);
    }

    [HttpGet("obtenerTodos/")]             // GET /Medico/obtenerTodos?filtro=garcia
    public async Task<IActionResult> ObtenerTodos([FromQuery] FiltroMedicoDto query)
    {
        var result = await _medicoService.ObtenerTodosAsync(query.Filtro);
        return Ok(result);
    }

    [HttpPost("agregar/")]                 // POST /Medico/agregar  (body: JSON)
    public async Task<IActionResult> Agregar(MedicoDto medico)
    {
        var response = await _medicoService.AgregarAsync(medico);
        return Ok(response);
    }

    [HttpPut("modificar/")]                // PUT /Medico/modificar
    public async Task<IActionResult> Modificar(MedicoDto medico)
    {
        var response = await _medicoService.ModificarAsync(medico);
        return Ok(response);
    }

    [HttpDelete("eliminar/{id}")]          // DELETE /Medico/eliminar/5
    public async Task<IActionResult> Eliminar(int id)
    {
        var response = await _medicoService.EliminarAsync(id);
        return Ok(response);
    }
}
```

**Lo que debes saber:**

| Atributo | Origen del dato | Ejemplo |
|---|---|---|
| `[FromQuery]` | Query string de la URL | `/medicos?filtro=garcia` |
| `[FromBody]` | Cuerpo JSON del request | Default en POST/PUT |
| `[FromRoute]` | Segmento de la ruta | `/medicos/{id}` |

---

## Bloque 5: ASP.NET Core MVC

**Archivos clave:** `Presentacion/Controllers/MedicoController.cs`

### 5.1 Controlador MVC vs API

| MVC Controller | API Controller |
|---|---|
| Hereda de `Controller` | Hereda de `ControllerBase` |
| Retorna `View()`, `RedirectToAction()` | Retorna `Ok()`, `BadRequest()` |
| Maneja sesiones y cookies | Es stateless |
| Renderiza HTML | Retorna JSON |

### 5.2 Acciones GET y POST

```csharp
[HttpGet]
public async Task<IActionResult> GestionarMedico(int? medicoId,
    TipoOperacion tipoOperacion = TipoOperacion.AGREGAR)
{
    var model = new GestionarMedicoViewModel()
    {
        Medico = medicoId.HasValue
            ? await _medicoServiceWeb.ObtenerPorId(medicoId.Value)
            : new MedicoDto(),
        Especialidades = await _medicoServiceWeb.ObtenerEspecialidades(),
        TipoOperacion = tipoOperacion
    };
    return View(model); // renderiza la vista con el modelo
}

[HttpPost]
public async Task<IActionResult> GestionarMedico(GestionarMedicoViewModel model)
{
    MedicoValidator validator = new MedicoValidator();
    ValidationResult result = validator.Validate(model);

    var response = new ServiceResponse();

    if (result.IsValid)
    {
        switch (model.TipoOperacion)
        {
            case TipoOperacion.AGREGAR:
                response = await _medicoServiceWeb.Agregar(model.Medico);
                break;
            case TipoOperacion.MODIFICAR:
                response = await _medicoServiceWeb.Modificar(model.Medico);
                break;
        }

        if (response.IsSuccess)
            return RedirectToAction("Index");
    }

    model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
    model.RespuestaServidor = response;
    return View(model);
}
```

### 5.3 ViewModels

Los ViewModels agrupan todo lo que la vista necesita:

```csharp
public class GestionarMedicoViewModel
{
    public MedicoDto Medico { get; set; }
    public List<EspecialidadDto> Especialidades { get; set; }  // para el dropdown
    public TipoOperacion TipoOperacion { get; set; }           // AGREGAR o MODIFICAR
    public ServiceResponse RespuestaServidor { get; set; }     // mensajes de error/éxito
}
```

### 5.4 Servicios Web (clientes HTTP a la API)

`Presentacion` no llama a la BD directamente. Llama a la API via HTTP:

```csharp
// Presentacion/Services/MedicoServiceWeb.cs
public class MedicoServiceWeb : IMedicoServiceWeb
{
    private readonly HttpClientService _httpClientService;

    public MedicoServiceWeb(HttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }

    public async Task<List<MedicoDto>> ObtenerTodos(string? filtro)
    {
        string query = QueryStringBuilder.ToQueryString(new { filtro });
        return await _httpClientService.GetAsync<List<MedicoDto>>($"Medico/obtenerTodos/{query}");
    }

    public async Task<ServiceResponse> Agregar(MedicoDto medico)
    {
        return await _httpClientService.PostAsync<MedicoDto, ServiceResponse>("Medico/agregar", medico);
    }
}
```

---

## Bloque 6: Autenticación y Autorización

### 6.1 Sesiones

```csharp
// Guardar al hacer login
HttpContext.Session.SetInt32("Sesion_UsuarioId", usuario.UsuarioId);
HttpContext.Session.SetString("Sesion_UsuarioNombre", usuario.Nombre);
HttpContext.Session.SetInt32("Sesion_UsuarioTipo", (int)usuario.TipoUsuario);

// Leer en cualquier lugar
var usuarioId = HttpContext.Session.GetInt32("Sesion_UsuarioId");
var nombre = HttpContext.Session.GetString("Sesion_UsuarioNombre");
```

### 6.2 Custom Authorization Filter (Control de acceso por roles)

**Archivo clave:** `Presentacion/Attributes/RoleAuthorizationAttribute.cs`

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RoleAuthorizationAttribute : Attribute, IAuthorizationFilter
{
    private readonly TipoUsuarioDto[] _allowedRoles;

    public RoleAuthorizationAttribute(params TipoUsuarioDto[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var session = context.HttpContext.Session;
        var usuarioId = session.GetInt32("Sesion_UsuarioId");
        var tipoUsuarioInt = session.GetInt32("Sesion_UsuarioTipo");

        // Sin sesión → redirigir al login
        if (!usuarioId.HasValue || !tipoUsuarioInt.HasValue)
        {
            context.Result = new RedirectToActionResult("Index", "Login", null);
            return;
        }

        var tipoUsuario = (TipoUsuarioDto)tipoUsuarioInt.Value;

        // Rol no permitido → acceso denegado
        if (!_allowedRoles.Contains(tipoUsuario))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
        }
    }
}
```

**Uso:**
```csharp
[RoleAuthorization(TipoUsuarioDto.ADMINISTRADOR, TipoUsuarioDto.MEDICO)]
public class MedicoController : Controller { ... }

// Los roles disponibles son:
// ADMINISTRADOR = 1
// MEDICO = 2
// PACIENTE = 3
```

### 6.3 Google OAuth

**Archivo clave:** `Presentacion/Controllers/LoginController.cs`

**Flujo:**
1. Usuario hace clic en "Ingresar con Google"
2. `GoogleLogin()` → redirige a Google con `Challenge()`
3. Google autentica y redirige a `/Login/GoogleCallback`
4. `GoogleCallback()` extrae claims (email, nombre, GoogleId)
5. Si el usuario existe en BD → login directo y guardar sesión
6. Si no existe → guardar datos en sesión y redirigir a completar registro

```csharp
public IActionResult GoogleLogin()
{
    var properties = new AuthenticationProperties { RedirectUri = "/Login/GoogleCallback" };
    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
}

public async Task<IActionResult> GoogleCallback()
{
    var result = await HttpContext.AuthenticateAsync("Cookies");
    var claims = result.Principal.Claims;

    var googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

    var usuarioExistente = await _usuarioServiceWeb.ObtenerPorGoogleId(googleId);

    if (usuarioExistente.Data != null)
    {
        // Login directo
        HttpContext.Session.SetInt32("Sesion_UsuarioId", usuarioExistente.Data.UsuarioId.Value);
        return RedirectToAction("Index", "Home");
    }
    else
    {
        // Primer acceso → completar registro
        HttpContext.Session.SetString("GoogleRegister_GoogleId", googleId);
        HttpContext.Session.SetString("GoogleRegister_Email", email);
        return RedirectToAction("CompleteGoogleRegistration", "Register");
    }
}
```

---

## Bloque 7: Middleware

**Archivo clave:** `Presentacion/Middleware/SessionValidationMiddleware.cs`

El middleware intercepta **todas las requests** antes de llegar al controlador:

```csharp
public class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;

    public SessionValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var publicPaths = new[] { "/Login", "/Home/AccessDenied", "/signin-google" };
        var currentPath = context.Request.Path.Value?.ToLower() ?? string.Empty;

        if (!publicPaths.Any(p => currentPath.StartsWith(p.ToLower())))
        {
            var usuarioId = context.Session.GetInt32("Sesion_UsuarioId");
            // lógica de validación de sesión
        }

        await _next(context); // pasar al siguiente eslabón del pipeline
    }
}

// Extensión para registrarlo limpiamente en Program.cs
public static class SessionValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseSessionValidation(this IApplicationBuilder builder)
        => builder.UseMiddleware<SessionValidationMiddleware>();
}
```

**En Program.cs:**
```csharp
app.UseSessionValidation(); // se ejecuta en cada request
```

**Lo que debes saber:** el middleware forma una "pipeline". Cada `await _next(context)` pasa al siguiente eslabón. Si no se llama, la request se corta.

---

## Bloque 8: FluentValidation

**Archivos clave:** `Presentacion/Tools/Validators/MedicoValidator.cs`

```csharp
public class MedicoValidator : AbstractValidator<GestionarMedicoViewModel>
{
    public MedicoValidator()
    {
        RuleFor(m => m.Medico.Apellido)
            .NotNull().WithMessage("El campo no debe ser nulo.")
            .NotEmpty().WithMessage("El campo Apellido es requerido.")
            .Matches("^[a-zA-Z\\s]*$").WithMessage("Solo letras y espacios.");

        RuleFor(m => m.Medico.Matricula)
            .NotEmpty().WithMessage("La Matrícula es requerida.")
            .Matches("^[a-zA-Z0-9/]*$").WithMessage("Solo letras y números.");

        RuleFor(m => m.Medico.EspecialidadId)
            .NotNull().WithMessage("La Especialidad es requerida.");
    }
}
```

**Uso en el controlador:**
```csharp
MedicoValidator validator = new MedicoValidator();
ValidationResult result = validator.Validate(model);

if (result.IsValid)
{
    // procesar
}
else
{
    LogicsForValidator.GetAllErrorsInView(ModelState, result); // pasar errores a la vista
    return View(model);
}
```

---

## Bloque 9: AutoMapper

**Archivo clave:** `Dominio/Application/Mappings/AutoMapperProfile.cs`

**Configuración (se hace una sola vez):**
```csharp
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Medico, MedicoDto>().ReverseMap();   // bidireccional
        CreateMap<Paciente, PacienteDto>().ReverseMap();
        CreateMap<OrdenMedica, OrdenMedicaDto>().ReverseMap();
        CreateMap<Turno, TurnoDto>().ReverseMap();
        // ...
    }
}
```

**Uso en servicios:**
```csharp
// Entidad → DTO
var medicoDto = _mapper.Map<MedicoDto>(medicoEntity);

// DTO → Entidad
var medico = _mapper.Map<Medico>(medicoDto);

// Lista completa
var lista = _mapper.Map<List<MedicoDto>>(listaMedicos);

// Actualizar un objeto existente (sin crear uno nuevo)
_mapper.Map(entity, ordenOriginal);
```

---

## Bloque 10: Flujo completo de una operación (Crear Médico)

Este es el recorrido completo de una acción desde el navegador hasta la base de datos:

```
1. Usuario abre /Medico/GestionarMedico
   → GET MedicoController.GestionarMedico() [Presentacion]

2. Controlador MVC llama al servicio web
   → IMedicoServiceWeb.ObtenerEspecialidades()

3. Servicio web hace HTTP GET a la API
   → GET http://api/Medico/obtenerEspecialidades/

4. Controlador API recibe el request
   → MedicoController.ObtenerEspecialidades() [PresentacionApi]

5. Controlador API llama al servicio de negocio
   → IMedicoService.ObtenerEspecialidadesAsync()

6. Servicio llama al repositorio
   → IMedicoRepository.ObtenerEspecialidadesAsync()

7. Repositorio consulta la BD
   → _context.Set<Especialidad>().ToListAsync()

8. Respuesta sube por todas las capas
   → BD → Repositorio → Servicio → API Controller → JSON
   → HttpClientService.GetAsync() → ServiceWeb → MVC Controller → View

9. Usuario llena el formulario y hace POST
   → POST MedicoController.GestionarMedico(model)

10. FluentValidation valida el formulario
    → Si falla: vuelve a la vista con errores

11. Servicio Web llama a la API
    → POST http://api/Medico/agregar  (body: MedicoDto JSON)

12. Servicio de negocio valida y guarda
    → Verifica matrícula duplicada
    → AutoMapper: MedicoDto → Medico
    → MedicoRepository.AddAsync(medico) → SQL INSERT

13. ServiceResponse sube de vuelta
    → IsSuccess → RedirectToAction("Index")
    → IsFailure → mostrar error en la vista
```

---

## Arquitectura del proyecto (resumen visual)

```
Presentacion (MVC - Razor Views)
    ↓ HTTP (HttpClientService)
PresentacionApi (REST API - Controllers)
    ↓ DI (IMedicoService)
Dominio/Servicios (Lógica de negocio)
    ↓ DI (IMedicoRepository)
Infraestructura/Repositorios (Acceso a datos)
    ↓ EF Core
SQL Server
```

---

## Preguntas trampa típicas de examen

**"¿Por qué hay dos carpetas de DTOs?"**

Porque `Presentacion` no referencia `Dominio` directamente. Son proyectos independientes que se comunican via HTTP/JSON. Si `Presentacion` usara los DTOs de `Dominio`, estarías acoplando capas que deben ser independientes.

**"¿Cuál es la diferencia entre `AddTransient`, `AddScoped` y `AddSingleton`?"**

- `Transient`: nueva instancia cada vez → ideal para servicios sin estado
- `Scoped`: una instancia por request HTTP → ideal para DbContext
- `Singleton`: una instancia para toda la vida de la app → configuraciones globales

**"¿Por qué se usa el patrón Repository si EF ya es una abstracción?"**

Para aislar el dominio del ORM. Si mañana cambias EF por Dapper, solo cambias los repositorios. El dominio y los servicios no se tocan.

**"¿Qué pasa si no llamo `await _next(context)` en un middleware?"**

La request se interrumpe. No llega al controlador ni a ningún middleware posterior.

**"¿Por qué ServiceResponse nunca lanza excepciones?"**

Las excepciones son para errores inesperados del sistema. Los errores de negocio (matrícula duplicada, orden ya entregada) son flujo esperado y se manejan con `response.AddError()`. Esto hace el código más predecible y los errores más fáciles de mostrar al usuario.

---

## Orden de estudio recomendado

| Semana | Tema |
|---|---|
| 1 | Fundamentos C#: interfaces, genéricos, async/await, LINQ |
| 2 | Patrones: Repository, DI, DTOs, ServiceResponse |
| 3 | Acceso a datos: EF Core, DbContext, relaciones, Include |
| 4 | Web API: verbos HTTP, rutas, IActionResult |
| 5 | MVC: controladores, ViewModels, sesiones, servicios web |
| 6 | Transversal: Autenticación, Middleware, FluentValidation, AutoMapper |
