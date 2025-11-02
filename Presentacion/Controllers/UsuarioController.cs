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
    [RoleAuthorization(TipoUsuarioDto.ADMINISTRADOR)]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioServiceWeb _usuarioServiceWeb;

        public UsuarioController(IUsuarioServiceWeb usuarioServiceWeb)
        {
            _usuarioServiceWeb = usuarioServiceWeb;
        }

        public async Task<IActionResult> Index(string? responseReturn, string? nombre)
        {
            var response = Serialization.DeserializeResponse(responseReturn);

            var model = new UsuarioViewModel()
            {
                Usuarios = await _usuarioServiceWeb.ObtenerTodos(nombre),
                RespuestaServidor = response
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GestionarUsuario(int? usuarioId, TipoOperacion tipoOperacion = TipoOperacion.AGREGAR)
        {
            var model = new GestionarUsuarioViewModel()
            {
                Usuario = usuarioId.HasValue ? await _usuarioServiceWeb.ObtenerPorId(usuarioId.Value) : new UsuarioDto(),
                TiposUsuario = _usuarioServiceWeb.ObtenerTiposUsuario(),
                TipoOperacion = tipoOperacion
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GestionarUsuario(GestionarUsuarioViewModel model)
        {
            UsuarioValidator validator = new UsuarioValidator();
            ValidationResult result = validator.Validate(model);

            var response = new ServiceResponse();

            if (result.IsValid)
            {
                switch (model.TipoOperacion)
                {
                    case TipoOperacion.AGREGAR:
                        var agregarResponse = await _usuarioServiceWeb.Agregar(model.Usuario);
                        response.AddErrors(agregarResponse.Errors);
                        break;

                    case TipoOperacion.MODIFICAR:
                        response = await _usuarioServiceWeb.Modificar(model.Usuario);
                        break;

                    default:
                        response.AddError("No se proporcionó el tipo de operación");
                        break;
                }

                if (response.IsSuccess)
                    return RedirectToAction("Index");
            }

            LogicsForValidator.GetAllErrorsInView(ModelState, result);
            model.TiposUsuario = _usuarioServiceWeb.ObtenerTiposUsuario();
            model.RespuestaServidor = response;

            return View(model);
        }
    }
}
