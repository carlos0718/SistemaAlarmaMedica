using Dominio.Application.DTOs;
using Dominio.Shared;

namespace Dominio.Servicios.Turnos
{
    public interface ITurnoService
    {
        Task<TurnoDto> ObtenerPorIdAsync(int id);
        Task<List<TurnoDto>> ObtenerTodosAsync();
        Task<List<TurnoDto>> ObtenerTurnosPorPacienteAsync(int pacienteId);
        Task<List<TurnoDto>> ObtenerTurnosPorMedicoAsync(int medicoId);
        Task<ServiceResponse> AgregarAsync(TurnoDto entity);
        Task<ServiceResponse> ModificarAsync(TurnoDto entity);
        Task<ServiceResponse> EliminarAsync(int id);
    }
}
