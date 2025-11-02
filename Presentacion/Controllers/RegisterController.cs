using Microsoft.AspNetCore.Mvc;
using Presentacion.Core.DTOs;
using Presentacion.Models;
using Presentacion.Services;
using System.Security.Claims;

namespace Presentacion.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IUsuarioServiceWeb _usuarioServiceWeb;
        private readonly IPacienteServiceWeb _pacienteServiceWeb;
        private readonly IMedicoServiceWeb _medicoServiceWeb;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterController(
            IUsuarioServiceWeb usuarioServiceWeb,
            IPacienteServiceWeb pacienteServiceWeb,
            IMedicoServiceWeb medicoServiceWeb,
            IHttpContextAccessor httpContextAccessor)
        {
            _usuarioServiceWeb = usuarioServiceWeb;
            _pacienteServiceWeb = pacienteServiceWeb;
            _medicoServiceWeb = medicoServiceWeb;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Formulario de registro estándar (sin Google)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var model = new RegisterViewModel
            {
                TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE },
                Especialidades = await _medicoServiceWeb.ObtenerEspecialidades()
            };

            return View(model);
        }

        /// <summary>
        /// Procesar el formulario de registro estándar
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
                model.TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE };
                return View(model);
            }

            // Validar que el tipo de usuario no sea ADMINISTRADOR
            if (model.TipoUsuario == TipoUsuarioDto.ADMINISTRADOR)
            {
                ModelState.AddModelError("", "No puedes registrarte como Administrador.");
                model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
                model.TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE };
                return View(model);
            }

            // Verificar que el username no exista
            var existingUser = await _usuarioServiceWeb.ObtenerPorNombre(model.Username);
            if (existingUser.Data != null)
            {
                ModelState.AddModelError("Username", "Este nombre de usuario ya está en uso.");
                model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
                model.TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE };
                return View(model);
            }

            try
            {
                int? medicoId = null;
                int? pacienteId = null;

                // PASO 1: Crear primero Médico o Paciente para obtener su ID
                if (model.TipoUsuario == TipoUsuarioDto.PACIENTE)
                {
                    // Crear Paciente primero
                    var paciente = new PacienteDto
                    {
                        Documento = int.TryParse(model.DocumentoPaciente, out int doc) ? doc : 0,
                        Apellido = model.ApellidoPaciente,
                        Nombre = model.NombrePaciente,
                        FechaNacimiento = null // Opcional: agregar a formulario si es necesario
                    };

                    var responsePaciente = await _pacienteServiceWeb.Agregar(paciente);
                    if (!responsePaciente.IsSuccess)
                    {
                        ModelState.AddModelError("", "Error al crear registro de paciente: " + responsePaciente.GetErrorsAsString());
                        model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
                        model.TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE };
                        return View(model);
                    }

                    // Obtener el paciente recién creado para obtener su ID
                    var pacientes = await _pacienteServiceWeb.ObtenerTodos(null);
                    var pacienteCreado = pacientes?.FirstOrDefault(p =>
                        p.Documento == paciente.Documento &&
                        p.Apellido == paciente.Apellido &&
                        p.Nombre == paciente.Nombre);

                    if (pacienteCreado?.PacienteId != null)
                    {
                        pacienteId = pacienteCreado.PacienteId.Value;
                    }
                }
                else if (model.TipoUsuario == TipoUsuarioDto.MEDICO)
                {
                    // Crear Médico primero
                    var medico = new MedicoDto
                    {
                        Apellido = model.ApellidoMedico,
                        Nombre = model.NombreMedico,
                        Matricula = model.MatriculaMedico,
                        EspecialidadId = model.EspecialidadId
                    };

                    var responseMedico = await _medicoServiceWeb.Agregar(medico);
                    if (!responseMedico.IsSuccess)
                    {
                        ModelState.AddModelError("", "Error al crear registro de médico: " + responseMedico.GetErrorsAsString());
                        model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
                        model.TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE };
                        return View(model);
                    }

                    // Obtener el médico recién creado para obtener su ID
                    var medicos = await _medicoServiceWeb.ObtenerTodos(null);
                    var medicoCreado = medicos?.FirstOrDefault(m =>
                        m.Matricula == medico.Matricula &&
                        m.Apellido == medico.Apellido &&
                        m.Nombre == medico.Nombre);

                    if (medicoCreado?.MedicoId != null)
                    {
                        medicoId = medicoCreado.MedicoId.Value;
                    }
                }

                // PASO 2: Crear usuario con la referencia al Médico o Paciente
                var nuevoUsuario = new UsuarioDto
                {
                    Nombre = model.Username,
                    Contrasena = model.Password,
                    TipoUsuario = model.TipoUsuario,
                    Activo = true,
                    Email = model.Email,
                    MedicoId = medicoId,      // Asignar MedicoId si es médico
                    PacienteId = pacienteId   // Asignar PacienteId si es paciente
                };

                var responseUsuario = await _usuarioServiceWeb.Agregar(nuevoUsuario);

                if (!responseUsuario.IsSuccess)
                {
                    // Si falla usuario, intentar eliminar el médico/paciente creado
                    if (medicoId.HasValue)
                    {
                        await _medicoServiceWeb.Eliminar(medicoId.Value);
                    }
                    if (pacienteId.HasValue)
                    {
                        await _pacienteServiceWeb.Eliminar(pacienteId.Value);
                    }

                    ModelState.AddModelError("", "Error al crear el usuario: " + responseUsuario.GetErrorsAsString());
                    model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
                    model.TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE };
                    return View(model);
                }

                // Registro exitoso
                TempData["SuccessMessage"] = "Registro completado exitosamente. Por favor, inicia sesión con tu usuario y contraseña o con Google.";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error en el registro: " + ex.Message);
                model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
                model.TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE };
                return View(model);
            }
        }

        /// <summary>
        /// Formulario para completar registro después de autenticación con Google
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CompleteGoogleRegistration()
        {
            // Obtener datos temporales de Google desde Session
            var googleId = _httpContextAccessor.HttpContext?.Session.GetString("GoogleRegister_GoogleId");
            var email = _httpContextAccessor.HttpContext?.Session.GetString("GoogleRegister_Email");

            if (string.IsNullOrEmpty(googleId) || string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Login", new { errorMessage = "Sesión de registro expirada. Por favor, intenta nuevamente." });
            }

            var model = new CompleteGoogleRegistrationViewModel
            {
                Email = email,
                TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE },
                Especialidades = await _medicoServiceWeb.ObtenerEspecialidades()
            };

            return View(model);
        }

        /// <summary>
        /// Procesar el formulario de completar registro de Google
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CompleteGoogleRegistration(CompleteGoogleRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE };
                model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
                return View(model);
            }

            // Validar que el tipo de usuario no sea ADMINISTRADOR
            if (model.TipoUsuario == TipoUsuarioDto.ADMINISTRADOR)
            {
                ModelState.AddModelError("", "No puedes registrarte como Administrador.");
                model.TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE };
                model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
                return View(model);
            }

            // Obtener datos de Google desde Session
            var googleId = _httpContextAccessor.HttpContext?.Session.GetString("GoogleRegister_GoogleId");
            var email = _httpContextAccessor.HttpContext?.Session.GetString("GoogleRegister_Email");

            if (string.IsNullOrEmpty(googleId) || string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Login", new { errorMessage = "Sesión de registro expirada. Por favor, intenta nuevamente." });
            }

            // Verificar que el username no exista
            var existingUser = await _usuarioServiceWeb.ObtenerPorNombre(model.Username);
            if (existingUser.Data != null)
            {
                ModelState.AddModelError("Username", "Este nombre de usuario ya está en uso.");
                model.TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE };
                model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
                return View(model);
            }

            try
            {
                int? medicoId = null;
                int? pacienteId = null;

                // PASO 1: Crear primero Médico o Paciente para obtener su ID
                if (model.TipoUsuario == TipoUsuarioDto.PACIENTE)
                {
                    // Crear Paciente primero
                    var paciente = new PacienteDto
                    {
                        Documento = int.TryParse(model.DocumentoPaciente, out int doc) ? doc : 0,
                        Apellido = model.ApellidoPaciente,
                        Nombre = model.NombrePaciente,
                        FechaNacimiento = model.FechaNacimientoPaciente
                    };

                    var responsePaciente = await _pacienteServiceWeb.Agregar(paciente);
                    if (!responsePaciente.IsSuccess)
                    {
                        ModelState.AddModelError("", "Error al crear registro de paciente: " + responsePaciente.GetErrorsAsString());
                        model.TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE };
                        model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
                        return View(model);
                    }

                    // Obtener el paciente recién creado para obtener su ID
                    var pacientes = await _pacienteServiceWeb.ObtenerTodos(null);
                    var pacienteCreado = pacientes?.FirstOrDefault(p =>
                        p.Documento == paciente.Documento &&
                        p.Apellido == paciente.Apellido &&
                        p.Nombre == paciente.Nombre);

                    if (pacienteCreado?.PacienteId != null)
                    {
                        pacienteId = pacienteCreado.PacienteId.Value;
                    }
                }
                else if (model.TipoUsuario == TipoUsuarioDto.MEDICO)
                {
                    // Crear Médico primero
                    var medico = new MedicoDto
                    {
                        Apellido = model.ApellidoMedico,
                        Nombre = model.NombreMedico,
                        Matricula = model.MatriculaMedico,
                        EspecialidadId = model.EspecialidadId
                    };

                    var responseMedico = await _medicoServiceWeb.Agregar(medico);
                    if (!responseMedico.IsSuccess)
                    {
                        ModelState.AddModelError("", "Error al crear registro de médico: " + responseMedico.GetErrorsAsString());
                        model.TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE };
                        model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
                        return View(model);
                    }

                    // Obtener el médico recién creado para obtener su ID
                    var medicos = await _medicoServiceWeb.ObtenerTodos(null);
                    var medicoCreado = medicos?.FirstOrDefault(m =>
                        m.Matricula == medico.Matricula &&
                        m.Apellido == medico.Apellido &&
                        m.Nombre == medico.Nombre);

                    if (medicoCreado?.MedicoId != null)
                    {
                        medicoId = medicoCreado.MedicoId.Value;
                    }
                }

                // PASO 2: Crear usuario con la referencia al Médico o Paciente
                var nuevoUsuario = new UsuarioDto
                {
                    Nombre = model.Username,
                    Contrasena = model.Password,
                    TipoUsuario = model.TipoUsuario,
                    Activo = true,
                    GoogleId = googleId,
                    Email = email,
                    MedicoId = medicoId,      // Asignar MedicoId si es médico
                    PacienteId = pacienteId   // Asignar PacienteId si es paciente
                };

                var response = await _usuarioServiceWeb.Agregar(nuevoUsuario);

                if (!response.IsSuccess)
                {
                    // Si falla usuario, intentar eliminar el médico/paciente creado
                    if (medicoId.HasValue)
                    {
                        await _medicoServiceWeb.Eliminar(medicoId.Value);
                    }
                    if (pacienteId.HasValue)
                    {
                        await _pacienteServiceWeb.Eliminar(pacienteId.Value);
                    }

                    ModelState.AddModelError("", response.GetErrorsAsString());
                    model.TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE };
                    model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
                    return View(model);
                }

                // Limpiar session de registro
                _httpContextAccessor.HttpContext?.Session.Remove("GoogleRegister_GoogleId");
                _httpContextAccessor.HttpContext?.Session.Remove("GoogleRegister_Email");
                _httpContextAccessor.HttpContext?.Session.Remove("GoogleRegister_Nombre");

                // Redirigir al login para que se autentique con Google nuevamente
                // Esto asegura que se carguen todos los datos correctamente desde la BD
                TempData["SuccessMessage"] = "Registro completado exitosamente. Por favor, inicia sesión con tu usuario y contraseña o con Google.";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error en el registro: " + ex.Message);
                model.TiposUsuario = new List<TipoUsuarioDto> { TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE };
                model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
                return View(model);
            }
        }
    }
}
