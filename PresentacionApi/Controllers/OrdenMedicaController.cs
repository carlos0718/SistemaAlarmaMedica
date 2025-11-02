using Dominio.Application.DTOs;
using Dominio.Servicios.OrdenesMedicas;
using Microsoft.AspNetCore.Mvc;
using PresentacionApi.Tools.FiltersDto;

namespace PresentacionApi.Controllers
{
    [Route("OrdenMedica")]
    [ApiController]
    public class OrdenMedicaController : ControllerBase
    {
        private readonly IOrdenMedicaService _ordenMedicaService;

        public OrdenMedicaController(IOrdenMedicaService ordenMedicaService)
        {
            _ordenMedicaService = ordenMedicaService;
        }

        [HttpGet("obtenerPorId/{id}", Name = "BuscarOrdenMedicaPorId")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _ordenMedicaService.ObtenerPorIdAsync(id);
            return Ok(result);
        }

        [HttpGet("obtenerTodos/", Name = "ObtenerTodosLasOrdenesMedicas")]
        public async Task<IActionResult> ObtenerTodos([FromQuery] FiltroOrdenMedicaDto query)
        {
            var result = await _ordenMedicaService.ObtenerTodosAsync(query.Filtro);
            return Ok(result);
        }

        [HttpPost("agregar/", Name = "AgregarOrdenMedica")]
        public async Task<IActionResult> Agregar(OrdenMedicaDto ordenMedica)
        {
            var response = await _ordenMedicaService.AgregarAsync(ordenMedica);
            return Ok(response);
        }

        [HttpPut("modificar/", Name = "ModificarOrdenMedica")]
        public async Task<IActionResult> Modificar(OrdenMedicaDto ordenMedica)
        {
            var response = await _ordenMedicaService.ModificarAsync(ordenMedica);
            return Ok(response);
        }

        [HttpDelete("eliminar/{id}", Name = "EliminarOrdenMedica")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var response = await _ordenMedicaService.EliminarAsync(id);
            return Ok(response);
        }

        [HttpPut("tomarOrdenMedica/", Name = "TomarOrdenMedica")]
        public async Task<IActionResult> TomarOrdenMedica(int ordenMedicaId)
        {
            var response = await _ordenMedicaService.TomarOrdenMedica(ordenMedicaId);
            return Ok(response);
        }

        [HttpGet("obtenerPorDni/", Name = "ObtenerOrdenesMedicasPorDni")]
        public async Task<IActionResult> ObtenerPorDni(int dni)
        {
            var result = await _ordenMedicaService.ObtenerPorDniAsync(dni);
            return Ok(result);
        }

        [HttpPut("empezarTratamientoLineaOrden/", Name = "EmpezarTratamientoDeLineaOrden")]
        public async Task<IActionResult> EmpezarTratamientoLineaOrden(int lineaOrdenMedicaId)
        {
            var result = await _ordenMedicaService.EmpezarTratamientoLineaOrdenMedicaAsync(lineaOrdenMedicaId);
            return Ok(result);
        }
    }
}
