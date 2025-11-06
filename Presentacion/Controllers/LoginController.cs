using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentacion.Core.DTOs;
using Presentacion.Services;
using System.Security.Claims;

namespace Presentacion.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUsuarioServiceWeb _usuarioServiceWeb;
        private readonly IMedicoServiceWeb _medicoServiceWeb;

        public LoginController(
            IHttpContextAccessor httpContextAccessor,
            IUsuarioServiceWeb usuarioServiceWeb,
            IMedicoServiceWeb medicoServiceWeb)
        {
            _httpContextAccessor = httpContextAccessor;
            _usuarioServiceWeb = usuarioServiceWeb;
            _medicoServiceWeb = medicoServiceWeb;
        }

        public IActionResult Index(string errorMessage = "")
        {
            ViewBag.MensajeError = errorMessage;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string nombre, string contrasena)
        {
            try
            {
                var response = await _usuarioServiceWeb.ObtenerPorNombreYContrasena(nombre, contrasena);

                if (response.IsSuccess && response.Data != null)
                {
                    var usuarioDto = response.Data;

                    _httpContextAccessor.HttpContext?.Session.SetInt32("Sesion_UsuarioId", (int)(usuarioDto?.UsuarioId));
                    _httpContextAccessor.HttpContext?.Session.SetString("Sesion_UsuarioNombre", usuarioDto?.Nombre);
                    _httpContextAccessor.HttpContext?.Session.SetInt32("Sesion_UsuarioTipo", (int)(usuarioDto?.TipoUsuario ?? TipoUsuarioDto.PACIENTE));
                    _httpContextAccessor.HttpContext?.Session.SetInt32("Timeout", 3);

                    // Si es paciente, guardar su PacienteId directamente desde el usuario
                    if (usuarioDto?.TipoUsuario == TipoUsuarioDto.PACIENTE && usuarioDto.PacienteId.HasValue)
                    {
                        _httpContextAccessor.HttpContext?.Session.SetInt32("Sesion_PacienteId", usuarioDto.PacienteId.Value);
                    }

                    // Si es médico, guardar su MedicoId y nombre
                    if (usuarioDto?.TipoUsuario == TipoUsuarioDto.MEDICO && usuarioDto.MedicoId.HasValue)
                    {
                        _httpContextAccessor.HttpContext?.Session.SetInt32("Sesion_MedicoId", usuarioDto.MedicoId.Value);

                        // Obtener información del médico para guardar su nombre
                        var medicoDto = await _medicoServiceWeb.ObtenerPorId(usuarioDto.MedicoId.Value);
                        if (medicoDto != null)
                        {
                            var nombreMedicoCompleto = $"{medicoDto.Apellido}, {medicoDto.Nombre} || Matric: {medicoDto.Matricula}";
                            _httpContextAccessor.HttpContext?.Session.SetString("Sesion_NombreMedico", nombreMedicoCompleto);
                        }
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Login", new { errorMessage = response.GetErrorsAsString()});
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login", new { errorMessage = ex.Message });
            }
        }

        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Remove("Sesion_UsuarioId");
            _httpContextAccessor.HttpContext?.Session.Remove("Sesion_UsuarioNombre");
            _httpContextAccessor.HttpContext?.Session.Remove("Sesion_UsuarioTipo");
            _httpContextAccessor.HttpContext?.Session.Remove("Sesion_PacienteId");
            _httpContextAccessor.HttpContext?.Session.Remove("Sesion_MedicoId");
            _httpContextAccessor.HttpContext?.Session.Remove("Sesion_NombreMedico");
            _httpContextAccessor.HttpContext?.Session.Remove("Timeout");

            return RedirectToAction("Index", "Login");
        }

        /// <summary>
        /// Inicia el flujo de autenticación con Google
        /// </summary>
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/Login/GoogleCallback",
                AllowRefresh = true
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Callback de Google después de la autenticación
        /// El middleware de Google procesa /signin-google y luego redirige aquí
        /// </summary>
        public async Task<IActionResult> GoogleCallback()
        {
            try
            {
                // El middleware ya procesó la autenticación, ahora leemos los claims del esquema de Cookies
                var result = await HttpContext.AuthenticateAsync("Cookies");

                if (!result.Succeeded || result.Principal == null)
                {
                    return RedirectToAction("Index", "Login", new { errorMessage = "Error al autenticar con Google. Intenta nuevamente." });
                }

                var claims = result.Principal.Claims;
                if (claims == null || !claims.Any())
                {
                    return RedirectToAction("Index", "Login", new { errorMessage = "No se pudieron obtener los datos de Google." });
                }

                // Extraer información del usuario de Google
                var googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var nombre = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(googleId) || string.IsNullOrEmpty(email))
                {
                    return RedirectToAction("Index", "Login", new { errorMessage = "Información de Google incompleta." });
                }

                // Buscar si el usuario ya existe por GoogleId
                var usuarioExistente = await _usuarioServiceWeb.ObtenerPorGoogleId(googleId);

                if (usuarioExistente.Data != null)
                {
                    // Usuario ya existe - Hacer login directo
                    var usuario = usuarioExistente.Data;

                    _httpContextAccessor.HttpContext?.Session.SetInt32("Sesion_UsuarioId", usuario.UsuarioId ?? 0);
                    _httpContextAccessor.HttpContext?.Session.SetString("Sesion_UsuarioNombre", usuario.Nombre ?? string.Empty);
                    _httpContextAccessor.HttpContext?.Session.SetInt32("Sesion_UsuarioTipo", (int)(usuario.TipoUsuario ?? TipoUsuarioDto.PACIENTE));
                    _httpContextAccessor.HttpContext?.Session.SetInt32("Timeout", 3);

                    // Si es paciente, guardar su PacienteId directamente desde el usuario
                    if (usuario?.TipoUsuario == TipoUsuarioDto.PACIENTE && usuario.PacienteId.HasValue)
                    {
                        _httpContextAccessor.HttpContext?.Session.SetInt32("Sesion_PacienteId", usuario.PacienteId.Value);
                    }

                    // Si es médico, guardar su MedicoId y nombre
                    if (usuario?.TipoUsuario == TipoUsuarioDto.MEDICO && usuario.MedicoId.HasValue)
                    {
                        _httpContextAccessor.HttpContext?.Session.SetInt32("Sesion_MedicoId", usuario.MedicoId.Value);

                        // Obtener información del médico para guardar su nombre
                        var medicoDto = await _medicoServiceWeb.ObtenerPorId(usuario.MedicoId.Value);
                        if (medicoDto != null)
                        {
                            var nombreMedicoCompleto = $"{medicoDto.Apellido}, {medicoDto.Nombre} || Matric: {medicoDto.Matricula}";
                            _httpContextAccessor.HttpContext?.Session.SetString("Sesion_NombreMedico", nombreMedicoCompleto);
                        }
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Usuario NO existe - Guardar datos en Session y redirigir a formulario de registro
                    _httpContextAccessor.HttpContext?.Session.SetString("GoogleRegister_GoogleId", googleId);
                    _httpContextAccessor.HttpContext?.Session.SetString("GoogleRegister_Email", email);
                    _httpContextAccessor.HttpContext?.Session.SetString("GoogleRegister_Nombre", nombre ?? string.Empty);

                    return RedirectToAction("CompleteGoogleRegistration", "Register");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login", new { errorMessage = $"Error en el proceso de autenticación: {ex.Message}" });
            }
        }
    }
}
