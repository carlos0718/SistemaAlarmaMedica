using Dominio.Application.DTOs;
using Dominio.Entidades;
using Dominio.Servicios.Pacientes;
using Dominio.Shared;
using Microsoft.AspNetCore.Mvc;
using PresentacionApi.Tools.FiltersDto;

namespace PresentacionApi.Controllers
{
    [Route("Paciente")]
    [ApiController]
    public class PacienteController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;

        public PacienteController(IPacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        [HttpGet("obtenerPorId/{id}", Name = "BuscarPacientePorId")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _pacienteService.ObtenerPorIdAsync(id);
            return Ok(result);
        }

        [HttpGet("obtenerTodos/", Name = "ObtenerTodosLosPacientes")]
        public async Task<IActionResult> ObtenerTodos([FromQuery] FiltroOrdenMedicaDto query)
        {
            var result = await _pacienteService.ObtenerTodosAsync(query.Filtro);
            return Ok(result);
        }

        [HttpPost("agregar/", Name = "AgregarPaciente")]
        public async Task<IActionResult> Agregar(PacienteDto paciente)
        {
            var response = await _pacienteService.AgregarAsync(paciente);
            return Ok(response);
        }

        [HttpPut("modificar/", Name = "ModificarPaciente")]
        public async Task<IActionResult> Modificar(PacienteDto paciente)
        {
            var response = await _pacienteService.ModificarAsync(paciente);
            return Ok(response);
        }

        [HttpDelete("eliminar/{id}", Name = "EliminarPaciente")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var response = await _pacienteService.EliminarAsync(id);
            return Ok(response);
        }

        [HttpPost("agregarOModificar/", Name = "AgregarOModificarPaciente")]
        public async Task<IActionResult> AgregarOModificar(PacienteDto paciente)
        {
            var response = new ServiceResponse();
            var pacienteDb = await _pacienteService.ExisteDocumentoAsync(paciente.Documento.Value);

            if (pacienteDb == null)
                response = await _pacienteService.AgregarAsync(paciente);
            else
            {
                paciente.PacienteId = pacienteDb.PacienteId;
                response = await _pacienteService.ModificarAsync(paciente);
            }

            return Ok(response);
        }
    }
}
