using Dominio.Application.DTOs;
using Dominio.Servicios.Medicos;
using Microsoft.AspNetCore.Mvc;
using PresentacionApi.Tools.FiltersDto;

namespace PresentacionApi.Controllers
{
    [Route("Medico")]
    [ApiController]
    public class MedicoController : ControllerBase
    {
        private readonly IMedicoService _medicoService;

        public MedicoController(IMedicoService medicoService)
        {
            _medicoService = medicoService;
        }

        #region MEDICO

        [HttpGet("obtenerPorId/{id}", Name = "BuscarMedicoPorId")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _medicoService.ObtenerPorIdAsync(id);
            return Ok(result);
        }

        [HttpGet("obtenerTodos/", Name = "ObtenerTodosLosMedicos")]
        public async Task<IActionResult> ObtenerTodos([FromQuery] FiltroMedicoDto query)
        {
            var result = await _medicoService.ObtenerTodosAsync(query.Filtro);
            return Ok(result);
        }

        [HttpPost("agregar/", Name = "AgregarMedico")]
        public async Task<IActionResult> Agregar(MedicoDto medico)
        {
            var response = await _medicoService.AgregarAsync(medico);
            return Ok(response);
        }

        [HttpPut("modificar/", Name = "ModificarMedico")]
        public async Task<IActionResult> Modificar(MedicoDto medico)
        {
            var response = await _medicoService.ModificarAsync(medico);
            return Ok(response);
        }

        [HttpDelete("eliminar/{id}", Name = "EliminarMedico")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var response = await _medicoService.EliminarAsync(id);
            return Ok(response);
        }

        #endregion MEDICO

        #region ESPECIALIDAD

        [HttpGet("obtenerEspecialidades/", Name = "ObtenerEspecialidades")]
        public async Task<IActionResult> ObtenerEspecialidades()
        {
            var result = await _medicoService.ObtenerEspecialidadesAsync();
            return Ok(result);
        }

        #endregion ESPECIALIDAD
    }
}
