using Microsoft.AspNetCore.Mvc;
using Presentacion.Attributes;
using Presentacion.Core.DTOs;
using Presentacion.Core;
using Presentacion.Models;
using Presentacion.Services;
using Presentacion.Tools.Serializations;
using Presentacion.Tools.Validators.Logic;
using Presentacion.Tools.Validators;
using FluentValidation.Results;
using Presentacion.Core.Responses;

namespace Presentacion.Controllers
{
    [RoleAuthorization(TipoUsuarioDto.ADMINISTRADOR, TipoUsuarioDto.MEDICO, TipoUsuarioDto.PACIENTE)]
    public class OrdenMedicaController : Controller
    {
        private readonly IOrdenMedicaServiceWeb _ordenMedicaServiceWeb;
        private readonly IMedicoServiceWeb _medicoServiceWeb;
        private readonly IPacienteServiceWeb _pacienteServiceWeb;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrdenMedicaController(
            IOrdenMedicaServiceWeb ordenMedicaServiceWeb,
            IMedicoServiceWeb medicoServiceWeb,
            IPacienteServiceWeb pacienteServiceWeb,
            IHttpContextAccessor httpContextAccessor)
        {
            _ordenMedicaServiceWeb = ordenMedicaServiceWeb;
            _medicoServiceWeb = medicoServiceWeb;
            _pacienteServiceWeb = pacienteServiceWeb;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(string? responseReturn, string? filtro)
        {
            var response = Serialization.DeserializeResponse(responseReturn);

            var model = new OrdenMedicaViewModel()
            {
                OrdenesMedicas = await _ordenMedicaServiceWeb.ObtenerTodos(filtro),
                RespuestaServidor = response
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GestionarOrdenMedica(int? ordenMedicaId, int? pacienteId, TipoOperacion tipoOperacion = TipoOperacion.AGREGAR)
        {
            // Obtener información del usuario de la sesión
            var tipoUsuarioInt = _httpContextAccessor.HttpContext?.Session.GetInt32("Sesion_UsuarioTipo");
            var tipoUsuario = (TipoUsuarioDto)(tipoUsuarioInt ?? (int)TipoUsuarioDto.PACIENTE);
            var medicoIdSession = _httpContextAccessor.HttpContext?.Session.GetInt32("Sesion_MedicoId");
            var nombreMedicoSession = _httpContextAccessor.HttpContext?.Session.GetString("Sesion_NombreMedico");

            var ordenMedica = ordenMedicaId.HasValue ? await _ordenMedicaServiceWeb.ObtenerPorId(ordenMedicaId.Value) : new OrdenMedicaDto();

            // Si se pasa un pacienteId, se pre-carga en la orden médica
            if (pacienteId.HasValue && !ordenMedicaId.HasValue)
            {
                ordenMedica.PacienteId = pacienteId.Value;
            }

            // Si el usuario es MÉDICO, pre-cargar su ID
            if (tipoUsuario == TipoUsuarioDto.MEDICO && medicoIdSession.HasValue && !ordenMedicaId.HasValue)
            {
                ordenMedica.MedicoId = medicoIdSession.Value;
            }

            var model = new GestionarOrdenMedicaViewModel()
            {
                OrdenMedica = ordenMedica,
                Medicos = await _medicoServiceWeb.ObtenerTodos(string.Empty),
                Pacientes = await _pacienteServiceWeb.ObtenerTodos(),
                ObrasSociales = _pacienteServiceWeb.ObtenerObrasSociales(),
                TipoOperacion = tipoOperacion,
                TipoUsuario = tipoUsuario,
                MedicoIdDelUsuario = medicoIdSession,
                NombreMedicoDelUsuario = nombreMedicoSession
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GestionarOrdenMedica(GestionarOrdenMedicaViewModel model)
        {
            // Obtener información del usuario de la sesión
            var tipoUsuarioInt = _httpContextAccessor.HttpContext?.Session.GetInt32("Sesion_UsuarioTipo");
            var tipoUsuario = (TipoUsuarioDto)(tipoUsuarioInt ?? (int)TipoUsuarioDto.PACIENTE);
            var medicoIdSession = _httpContextAccessor.HttpContext?.Session.GetInt32("Sesion_MedicoId");
            var nombreMedicoSession = _httpContextAccessor.HttpContext?.Session.GetString("Sesion_NombreMedico");

            if (model.OrdenMedica != null && model.OrdenMedica.LineaOrdenMedica != null && model.OrdenMedica.LineaOrdenMedica.Any())
            {
                model.OrdenMedica.LineaOrdenMedica = model.OrdenMedica.LineaOrdenMedica
               .Where(l => l != null && !string.IsNullOrWhiteSpace(l.NumeroRegistro))
               .ToList();
            }

            OrdenMedicaValidator validator = new OrdenMedicaValidator();
            ValidationResult result = validator.Validate(model);

            var response = new ServiceResponse();

            if (result.IsValid)
            {
                switch (model.TipoOperacion)
                {
                    case TipoOperacion.AGREGAR:
                        model.OrdenMedica.Fecha = DateTime.Now;
                        response = await _ordenMedicaServiceWeb.Agregar(model.OrdenMedica);
                        break;

                    case TipoOperacion.MODIFICAR:
                        response = await _ordenMedicaServiceWeb.Modificar(model.OrdenMedica);
                        break;

                    default:
                        response.AddError("No se proporcionó el tipo de operación");
                        break;
                }

                if (response.IsSuccess)
                    return RedirectToAction("Index");
            }

            LogicsForValidator.GetAllErrorsInView(ModelState, result);
            model.Medicos = await _medicoServiceWeb.ObtenerTodos();
            model.Pacientes = await _pacienteServiceWeb.ObtenerTodos();
            model.ObrasSociales = _pacienteServiceWeb.ObtenerObrasSociales();
            model.TipoUsuario = tipoUsuario;
            model.MedicoIdDelUsuario = medicoIdSession;
            model.NombreMedicoDelUsuario = nombreMedicoSession;

            model.RespuestaServidor = response;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int ordenMedicaId)
        {
            var response = await _ordenMedicaServiceWeb.Eliminar(ordenMedicaId);

            return RedirectToAction("Index", new { responseReturn = Serialization.SerializeResponse(response) });
        }
    }
}
