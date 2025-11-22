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
    [RoleAuthorization(TipoUsuarioDto.ADMINISTRADOR, TipoUsuarioDto.MEDICO)]
    public class PacienteController : Controller
    {
        private readonly IPacienteServiceWeb _pacienteServiceWeb;
        private readonly ITurnoServiceWeb _turnoServiceWeb;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PacienteController(
            IPacienteServiceWeb pacienteServiceWeb,
            ITurnoServiceWeb turnoServiceWeb,
            IHttpContextAccessor httpContextAccessor)
        {
            _pacienteServiceWeb = pacienteServiceWeb;
            _turnoServiceWeb = turnoServiceWeb;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(string? responseReturn, string? filtro)
        {
            var response = Serialization.DeserializeResponse(responseReturn);

            // Obtener tipo de usuario de la sesión
            var tipoUsuarioInt = _httpContextAccessor.HttpContext?.Session.GetInt32("Sesion_UsuarioTipo");
            var tipoUsuario = (TipoUsuarioDto)(tipoUsuarioInt ?? (int)TipoUsuarioDto.PACIENTE);

            var model = new PacienteViewModel()
            {
                RespuestaServidor = response,
                TipoUsuario = tipoUsuario
            };

            // Obtener todos los turnos (necesarios para médico y admin)
            var todosTurnos = await _turnoServiceWeb.ObtenerTodos();

            // Si es médico, obtener sus turnos PENDIENTES ordenados por fecha
            if (tipoUsuario == TipoUsuarioDto.MEDICO)
            {
                // Obtener el MedicoId directamente de la sesión
                var medicoId = _httpContextAccessor.HttpContext?.Session.GetInt32("Sesion_MedicoId");

                if (medicoId.HasValue && medicoId.Value > 0)
                {
                    // Filtrar y mostrar SOLO los turnos PENDIENTES del médico ordenados por fecha descendente
                    var pacientesConTurnos = todosTurnos
                        .Where(t => t.MedicoId == medicoId.Value && t.Estado == Core.DTOs.EstadoTurnoDto.PENDIENTE) // PENDIENTE
                        .Select(t => new PacienteConTurnoDto
                        {
                            Paciente = t.Paciente,
                            FechaTurno = t.FechaTurno,
                            TurnoId = t.TurnoId // Incluir TurnoId para trazabilidad
                        })
                        .OrderByDescending(p => p.FechaTurno) // Ordenar del más reciente al más antiguo
                        .ToList();

                    model.PacientesConTurnos = pacientesConTurnos;
                }
                else
                {
                    model.PacientesConTurnos = new List<PacienteConTurnoDto>();
                }
            }
            else
            {
                // Para admin, obtener todos los pacientes CON su fecha de turno más reciente
                var pacientes = await _pacienteServiceWeb.ObtenerTodos(filtro);

                // Crear lista de pacientes con turnos para el admin
                var pacientesConTurnos = pacientes
                    .Select(p => new PacienteConTurnoDto
                    {
                        Paciente = p,
                        FechaTurno = todosTurnos
                            .Where(t => t.PacienteId == p.PacienteId)
                            .OrderByDescending(t => t.FechaTurno)
                            .FirstOrDefault()?.FechaTurno
                    })
                    .ToList();

                model.PacientesConTurnos = pacientesConTurnos;
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GestionarPaciente(int? pacienteId, TipoOperacion tipoOperacion = TipoOperacion.AGREGAR)
        {
            // Obtener tipo de usuario de la sesión
            var tipoUsuarioInt = _httpContextAccessor.HttpContext?.Session.GetInt32("Sesion_UsuarioTipo");
            var tipoUsuario = (TipoUsuarioDto)(tipoUsuarioInt ?? (int)TipoUsuarioDto.PACIENTE);

            var model = new GestionarPacienteViewModel()
            {
                Paciente = pacienteId.HasValue ? await _pacienteServiceWeb.ObtenerPorId(pacienteId.Value) : new PacienteDto(),
                TipoOperacion = tipoOperacion,
                TipoUsuario = tipoUsuario
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GestionarPaciente(GestionarPacienteViewModel model)
        {
            PacienteValidator validator = new PacienteValidator();
            ValidationResult result = validator.Validate(model);

            var response = new ServiceResponse();

            if (result.IsValid)
            {
                switch (model.TipoOperacion)
                {
                    case TipoOperacion.AGREGAR:
                        response = await _pacienteServiceWeb.Agregar(model.Paciente);
                        break;

                    case TipoOperacion.MODIFICAR:
                        response = await _pacienteServiceWeb.Modificar(model.Paciente);
                        break;

                    default:
                        response.AddError("No se proporcionó el tipo de operación");
                        break;
                }

                if (response.IsSuccess)
                    return RedirectToAction("Index");
            }

            LogicsForValidator.GetAllErrorsInView(ModelState, result);

            model.RespuestaServidor = response;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int pacienteId)
        {
            var response = await _pacienteServiceWeb.Eliminar(pacienteId);

            return RedirectToAction("Index", new { responseReturn = Serialization.SerializeResponse(response) });
        }
    }
}
