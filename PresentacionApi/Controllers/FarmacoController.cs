using Dominio.Servicios.Farmacos;
using Microsoft.AspNetCore.Mvc;

namespace PresentacionApi.Controllers
{
    [Route("Farmaco")]
    [ApiController]
    public class FarmacoController : ControllerBase
    {
        private readonly IFarmacosService _farmacosService;

        public FarmacoController(IFarmacosService farmacosService)
        {
            _farmacosService = farmacosService;
        }

        [HttpGet("buscarPorNumeroRegistro/{numeroRegistro}", Name = "BuscarMedicamentoPorNumeroRegistro")]
        public async Task<IActionResult> BuscarPorNumeroRegistro(string numeroRegistro)
        {
            var result = await _farmacosService.BuscarPorNumeroRegistro(numeroRegistro);
            return Ok(result);
        }

        [HttpGet("buscarPorNombre/{nombre}", Name = "BuscarMedicamentosPorNombre")]
        public async Task<IActionResult> BuscarPorNombre(string nombre)
        {
            var result = await _farmacosService.BuscarMedicamentosPorNombre(nombre);
            return Ok(result);
        }

        [HttpGet("buscarPorPagina/{pagina}", Name = "BuscarMedicamentosPorPagina")]
        public async Task<IActionResult> BuscarPorPagina(int pagina)
        {
            var result = await _farmacosService.ObtenerMedicamentosPorPagina(pagina);
            return Ok(result);
        }
    }
}
