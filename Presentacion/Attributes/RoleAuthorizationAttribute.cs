using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Presentacion.Core.DTOs;

namespace Presentacion.Attributes
{
    /// <summary>
    /// Atributo personalizado para autorizar acceso basado en roles de usuario
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RoleAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        private readonly TipoUsuarioDto[] _allowedRoles;

        public RoleAuthorizationAttribute(params TipoUsuarioDto[] allowedRoles)
        {
            _allowedRoles = allowedRoles ?? throw new ArgumentNullException(nameof(allowedRoles));
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var session = httpContext.Session;

            // Verificar si existe sesión activa
            var usuarioId = session.GetInt32("Sesion_UsuarioId");
            var tipoUsuarioInt = session.GetInt32("Sesion_UsuarioTipo");

            // Si no hay sesión, redirigir al login
            if (!usuarioId.HasValue || !tipoUsuarioInt.HasValue)
            {
                context.Result = new RedirectToActionResult("Index", "Login", new { errorMessage = "Debe iniciar sesión para acceder a esta página" });
                return;
            }

            // Convertir el int a enum
            var tipoUsuario = (TipoUsuarioDto)tipoUsuarioInt.Value;

            // Verificar si el rol del usuario está permitido
            if (!_allowedRoles.Contains(tipoUsuario))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }
        }
    }
}
