using Microsoft.AspNetCore.Mvc;
using Presentacion.Attributes;
using Presentacion.Core.DTOs;
using Presentacion.Models;
using Presentacion.Services;

namespace Presentacion.Controllers
{
    [RoleAuthorization(TipoUsuarioDto.ADMINISTRADOR, TipoUsuarioDto.MEDICO)]
    public class FarmacoController : Controller
    {
        private readonly IFarmacoServiceWeb _farmacoServiceWeb;

        public FarmacoController(IFarmacoServiceWeb farmacoService)
        {
            _farmacoServiceWeb = farmacoService;
        }

        public async Task<IActionResult> Index(string nombre)
        {
            var farmacos = new List<FarmacoDto>();

            if (string.IsNullOrEmpty(nombre))
            {
                farmacos = await _farmacoServiceWeb.ObtenerFarmacosPorPagina(1);
            }
            else
            {
                farmacos = await _farmacoServiceWeb.ObtenerFarmacosPorNombre(nombre);
            }

            var model = new FarmacoViewModel()
            {
                Farmacos = farmacos
            };

            return View(model);
        }

        public async Task<IActionResult> BuscarPorNumeroRegistroONombre(string numeroRegistro, string nombre)
        {
            var farmacos = new List<FarmacoDto>();

            if (!string.IsNullOrEmpty(numeroRegistro))
            {
                var farmaco = await _farmacoServiceWeb.ObtenerFarmacosPorNumeroRegistro(numeroRegistro);
                if (farmaco != null) farmacos.Add(farmaco);
            }
            
            if (!string.IsNullOrEmpty(nombre))
            {
                farmacos = await _farmacoServiceWeb.ObtenerFarmacosPorNombre(nombre);
            }

            return Ok(farmacos);
        }

    }
}
