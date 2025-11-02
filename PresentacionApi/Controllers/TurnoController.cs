using Dominio.Application.DTOs;
using Dominio.Servicios.Medicos;
using Dominio.Servicios.Turnos;
using Dominio.Shared;
using Microsoft.AspNetCore.Mvc;

namespace PresentacionApi.Controllers
{
    [Route("Turno")]
    [ApiController]
    public class TurnoController : ControllerBase
    {
        private readonly ITurnoService _turnoService;
        private readonly IMedicoService _medicoService;

        public TurnoController(ITurnoService turnoService, IMedicoService medicoService)
        {
            _turnoService = turnoService;
            _medicoService = medicoService;
        }

        [HttpGet("obtenerPorId/{id}", Name = "BuscarTurnoPorId")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _turnoService.ObtenerPorIdAsync(id);
            return Ok(result);
        }

        [HttpGet("obtenerTodos/", Name = "ObtenerTodoLosTurnos")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var result = await _turnoService.ObtenerTodosAsync();
            return Ok(result);
        }

        [HttpGet("obtenerTurnosPorPaciente/{pacienteId}", Name = "ObtenerTurnosPorPaciente")]
        public async Task<IActionResult> ObtenerTurnosPorPaciente(int pacienteId)
        {
            var result = await _turnoService.ObtenerTurnosPorPacienteAsync(pacienteId);
            return Ok(result);
        }

        [HttpPost("agregar/", Name = "AgregarTurno")]
        public async Task<IActionResult> Agregar(TurnoDto turno)
        {
            var response = await _turnoService.AgregarAsync(turno);
            return Ok(response);
        }

        [HttpPut("modificar/", Name = "ModificarTurno")]
        public async Task<IActionResult> Modificar(TurnoDto turno)
        {
            var response = await _turnoService.ModificarAsync(turno);
            return Ok(response);
        }

        [HttpDelete("eliminar/{id}", Name = "EliminarTurno")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var response = await _turnoService.EliminarAsync(id);
            return Ok(response);
        }

        [HttpGet("obtenerMedicosPorEspecialidad/{especialidadId}", Name = "ObtenerMedicosPorEspecialidad")]
        public async Task<IActionResult> ObtenerMedicosPorEspecialidad(int especialidadId)
        {
            var result = await _medicoService.ObtenerMedicosPorEspecialidadAsync(especialidadId);
            return Ok(result);
        }

        [HttpGet("obtenerPacientesPorMedico/{medicoId}", Name = "ObtenerPacientesPorMedico")]
        public async Task<IActionResult> ObtenerPacientesPorMedico(int medicoId)
        {
            // Obtener turnos del médico con las relaciones incluidas
            var turnos = await _turnoService.ObtenerTurnosPorMedicoAsync(medicoId);

            // Obtener pacientes únicos de esos turnos
            var pacientes = turnos
                .Where(t => t.Paciente != null)
                .GroupBy(t => t.PacienteId)
                .Select(g => g.First().Paciente)
                .Distinct()
                .ToList();

            return Ok(pacientes);
        }
    }
}
