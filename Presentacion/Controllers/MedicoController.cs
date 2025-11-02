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
    public class MedicoController : Controller
    {
        private readonly IMedicoServiceWeb _medicoServiceWeb;

        public MedicoController(IMedicoServiceWeb medicoService)
        {
            _medicoServiceWeb = medicoService;
        }

        public async Task<IActionResult> Index(string? responseReturn, string? filtro)
        {
            var response = Serialization.DeserializeResponse(responseReturn);

            var model = new MedicoViewModel()
            {
                Medicos = await _medicoServiceWeb.ObtenerTodos(filtro),
                RespuestaServidor = response
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GestionarMedico(int? medicoId, TipoOperacion tipoOperacion = TipoOperacion.AGREGAR)
        {
            var model = new GestionarMedicoViewModel()
            {
                Medico = medicoId.HasValue ? await _medicoServiceWeb.ObtenerPorId(medicoId.Value) : new MedicoDto() ,
                Especialidades = await _medicoServiceWeb.ObtenerEspecialidades(),
                TipoOperacion = tipoOperacion
            };

            return View(model);
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

                    default:
                        response.AddError("No se proporcionó el tipo de operación");
                        break;
                }

                if (response.IsSuccess)
                    return RedirectToAction("Index");
            }

            LogicsForValidator.GetAllErrorsInView(ModelState, result);

            model.Especialidades = await _medicoServiceWeb.ObtenerEspecialidades();
            model.RespuestaServidor = response;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int medicoId)
        {
            var response = await _medicoServiceWeb.Eliminar(medicoId);

            return RedirectToAction("Index", new { responseReturn = Serialization.SerializeResponse(response) });
        }
    }
}
