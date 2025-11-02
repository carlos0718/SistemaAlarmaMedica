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

            List<PacienteDto> pacientes;

            // Si es médico, obtener solo sus pacientes (que tienen turnos con él)
            if (tipoUsuario == TipoUsuarioDto.MEDICO)
            {
                // Obtener el MedicoId directamente de la sesión
                var medicoId = _httpContextAccessor.HttpContext?.Session.GetInt32("Sesion_MedicoId");

                if (medicoId.HasValue && medicoId.Value > 0)
                {
                    pacientes = await _turnoServiceWeb.ObtenerPacientesPorMedico(medicoId.Value);
                }
                else
                {
                    pacientes = new List<PacienteDto>();
                }
            }
            else
            {
                // Para admin y paciente, obtener todos los pacientes
                pacientes = await _pacienteServiceWeb.ObtenerTodos(filtro);
            }

            var model = new PacienteViewModel()
            {
                Pacientes = pacientes,
                RespuestaServidor = response,
                TipoUsuario = tipoUsuario
            };

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
