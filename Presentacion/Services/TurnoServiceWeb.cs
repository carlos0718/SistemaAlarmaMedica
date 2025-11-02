using Presentacion.Core.DTOs;
using Presentacion.Core.Responses;

namespace Presentacion.Services
{
    public class TurnoServiceWeb : ITurnoServiceWeb
    {
        private readonly HttpClientService _httpClientService;

        public TurnoServiceWeb(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<TurnoDto> ObtenerPorId(int id)
        {
            return await _httpClientService.GetAsync<TurnoDto>($"Turno/obtenerPorId/{id}");
        }

        public async Task<List<TurnoDto>> ObtenerTodos()
        {
            return await _httpClientService.GetAsync<List<TurnoDto>>("Turno/obtenerTodos/");
        }

        public async Task<List<TurnoDto>> ObtenerTurnosPorPaciente(int pacienteId)
        {
            return await _httpClientService.GetAsync<List<TurnoDto>>($"Turno/obtenerTurnosPorPaciente/{pacienteId}");
        }

        public async Task<ServiceResponse> Agregar(TurnoDto turno)
        {
            return await _httpClientService.PostAsync<TurnoDto, ServiceResponse>("Turno/agregar", turno);
        }

        public async Task<ServiceResponse> Modificar(TurnoDto turno)
        {
            return await _httpClientService.PutAsync<TurnoDto, ServiceResponse>("Turno/modificar", turno);
        }

        public async Task<ServiceResponse> Eliminar(int id)
        {
            return await _httpClientService.DeleteAsync<ServiceResponse>($"Turno/eliminar/{id}");
        }

        public async Task<List<MedicoDto>> ObtenerMedicosPorEspecialidad(int especialidadId)
        {
            return await _httpClientService.GetAsync<List<MedicoDto>>($"Turno/obtenerMedicosPorEspecialidad/{especialidadId}");
        }

        public async Task<List<PacienteDto>> ObtenerPacientesPorMedico(int medicoId)
        {
            return await _httpClientService.GetAsync<List<PacienteDto>>($"Turno/obtenerPacientesPorMedico/{medicoId}");
        }
    }

    public interface ITurnoServiceWeb
    {
        Task<TurnoDto> ObtenerPorId(int id);
        Task<List<TurnoDto>> ObtenerTodos();
        Task<List<TurnoDto>> ObtenerTurnosPorPaciente(int pacienteId);
        Task<ServiceResponse> Agregar(TurnoDto turno);
        Task<ServiceResponse> Modificar(TurnoDto turno);
        Task<ServiceResponse> Eliminar(int id);
        Task<List<MedicoDto>> ObtenerMedicosPorEspecialidad(int especialidadId);
        Task<List<PacienteDto>> ObtenerPacientesPorMedico(int medicoId);
    }
}
