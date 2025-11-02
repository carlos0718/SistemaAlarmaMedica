namespace Presentacion.Middleware
{
    /// <summary>
    /// Middleware para validar sesiones activas y renovar timeout
    /// </summary>
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Rutas públicas que no requieren sesión
            var publicPaths = new[] { "/Login", "/login", "/Home/AccessDenied", "/home/accessdenied", "/signin-google" };
            var currentPath = context.Request.Path.Value?.ToLower() ?? string.Empty;

            // Si no es una ruta pública, verificar sesión
            if (!publicPaths.Any(p => currentPath.StartsWith(p.ToLower())))
            {
                var usuarioId = context.Session.GetInt32("Sesion_UsuarioId");
                var timeout = context.Session.GetInt32("Timeout");

                if (usuarioId.HasValue && timeout.HasValue)
                {
                    // Renovar timeout de sesión
                    context.Session.SetInt32("Timeout", timeout.Value);
                }
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extensión para facilitar el registro del middleware
    /// </summary>
    public static class SessionValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionValidationMiddleware>();
        }
    }
}
