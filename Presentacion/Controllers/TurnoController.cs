using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Presentacion.Attributes;
using Presentacion.Core;
using Presentacion.Core.DTOs;
using Presentacion.Core.Responses;
using Presentacion.Models;
using Presentacion.Services;
using Presentacion.Tools.Serializations;
using Presentacion.Tools.Validators;
using Presentacion.Tools.Validators.Logic;

namespace Presentacion.Controllers
{
    [RoleAuthorization(TipoUsuarioDto.ADMINISTRADOR, TipoUsuarioDto.PACIENTE)]
    public class TurnoController : Controller
    {
        private readonly ITurnoServiceWeb _turnoServiceWeb;
        private readonly IPacienteServiceWeb _pacienteServiceWeb;
        private readonly IMedicoServiceWeb _medicoServiceWeb;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TurnoController(
            ITurnoServiceWeb turnoServiceWeb,
            IPacienteServiceWeb pacienteServiceWeb,
            IMedicoServiceWeb medicoServiceWeb,
            IHttpContextAccessor httpContextAccessor)
        {
            _turnoServiceWeb = turnoServiceWeb;
            _pacienteServiceWeb = pacienteServiceWeb;
            _medicoServiceWeb = medicoServiceWeb;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(string? responseReturn)
        {
            var response = Serialization.DeserializeResponse(responseReturn);

            // Obtener tipo de usuario de la sesión
            var tipoUsuarioInt = _httpContextAccessor.HttpContext?.Session.GetInt32("Sesion_UsuarioTipo");
            var tipoUsuario = (TipoUsuarioDto)(tipoUsuarioInt ?? (int)TipoUsuarioDto.PACIENTE);

            // Si es paciente, redirigir directamente al formulario de reservar turno
            if (tipoUsuario == TipoUsuarioDto.PACIENTE)
            {
                return RedirectToAction("GestionarTurno");
            }

            // Solo administradores ven el listado de turnos
            var turnos = await _turnoServiceWeb.ObtenerTodos();

            var model = new TurnoViewModel()
            {
                Turnos = turnos,
                RespuestaServidor = response
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GestionarTurno(int? turnoId, TipoOperacion tipoOperacion = TipoOperacion.AGREGAR)
        {
            var tipoUsuarioInt = _httpContextAccessor.HttpContext?.Session.GetInt32("Sesion_UsuarioTipo");
            var tipoUsuario = (TipoUsuarioDto)(tipoUsuarioInt ?? (int)TipoUsuarioDto.PACIENTE);
            var pacienteIdSession = _httpContextAccessor.HttpContext?.Session.GetInt32("Sesion_PacienteId");

            // Obtener especialidades para el filtro
            var especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
            var medicos = await _medicoServiceWeb.ObtenerTodos(null);
            var pacientes = await _pacienteServiceWeb.ObtenerTodos(null);

            var turno = new TurnoDto();
            if (turnoId.HasValue)
            {
                turno = await _turnoServiceWeb.ObtenerPorId(turnoId.Value);
            }
            else if (tipoUsuario == TipoUsuarioDto.PACIENTE && pacienteIdSession.HasValue)
            {
                turno.PacienteId = pacienteIdSession.Value;
            }

            // Si es paciente, cargar sus turnos reservados
            List<TurnoDto> turnosPaciente = new List<TurnoDto>();
            if (tipoUsuario == TipoUsuarioDto.PACIENTE && pacienteIdSession.HasValue)
            {
                turnosPaciente = await _turnoServiceWeb.ObtenerTurnosPorPaciente(pacienteIdSession.Value);
            }

            // Obtener mensaje de confirmación si existe
            var mensajeConfirmacion = TempData["MensajeConfirmacion"] as string;

            var model = new GestionarTurnoViewModel()
            {
                Turno = turno,
                TipoOperacion = tipoOperacion,
                Especialidades = especialidades,
                Medicos = medicos,
                Pacientes = pacientes,
                RespuestaServidor = new ServiceResponse(),
                TipoUsuario = tipoUsuario,
                TurnosPaciente = turnosPaciente,
                MensajeConfirmacion = mensajeConfirmacion
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GestionarTurno(GestionarTurnoViewModel model)
        {
            TurnoValidator validator = new TurnoValidator();
            ValidationResult result = validator.Validate(model);

            var response = new ServiceResponse();

            // Obtener tipo de usuario
            var tipoUsuarioInt = _httpContextAccessor.HttpContext?.Session.GetInt32("Sesion_UsuarioTipo");
            var tipoUsuario = (TipoUsuarioDto)(tipoUsuarioInt ?? (int)TipoUsuarioDto.PACIENTE);
            var pacienteIdSession = _httpContextAccessor.HttpContext?.Session.GetInt32("Sesion_PacienteId");

            if (result.IsValid)
            {
                switch (model.TipoOperacion)
                {
                    case TipoOperacion.AGREGAR:
                        response = await _turnoServiceWeb.Agregar(model.Turno);
                        break;

                    case TipoOperacion.MODIFICAR:
                        response = await _turnoServiceWeb.Modificar(model.Turno);
                        break;

                    default:
                        response.AddError("No se proporcionó el tipo de operación");
                        break;
                }

                if (response.IsSuccess)
                {
                    // Si es paciente, redirigir a GestionarTurno con mensaje de confirmación
                    if (tipoUsuario == TipoUsuarioDto.PACIENTE)
                    {
                        TempData["MensajeConfirmacion"] = "¡Turno reservado exitosamente! Su turno ha sido confirmado.";
                        return RedirectToAction("GestionarTurno");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }

            LogicsForValidator.GetAllErrorsInView(ModelState, result);

            model.RespuestaServidor = response;
            model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
            model.Medicos = await _medicoServiceWeb.ObtenerTodos(null);
            model.Pacientes = await _pacienteServiceWeb.ObtenerTodos(null);
            model.TipoUsuario = tipoUsuario;

            // Si es paciente, recargar sus turnos
            if (tipoUsuario == TipoUsuarioDto.PACIENTE && pacienteIdSession.HasValue)
            {
                model.TurnosPaciente = await _turnoServiceWeb.ObtenerTurnosPorPaciente(pacienteIdSession.Value);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int turnoId)
        {
            var response = await _turnoServiceWeb.Eliminar(turnoId);
            return RedirectToAction("Index", new { responseReturn = Serialization.SerializeResponse(response) });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerMedicosPorEspecialidad(int especialidadId)
        {
            var medicos = await _turnoServiceWeb.ObtenerMedicosPorEspecialidad(especialidadId);
            return Json(medicos);
        }
    }
}
