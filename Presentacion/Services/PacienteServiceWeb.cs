using Presentacion.Core.DTOs;
using Presentacion.Core.Responses;
using Presentacion.Tools.QueryBuilder;

namespace Presentacion.Services
{
    public class PacienteServiceWeb : IPacienteServiceWeb
    {
        private readonly HttpClientService _httpClientService;

        public PacienteServiceWeb(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<PacienteDto> ObtenerPorId(int id)
        {
            return await _httpClientService.GetAsync<PacienteDto>($"Paciente/obtenerPorId/{id}");
        }

        public async Task<List<PacienteDto>> ObtenerTodos(string? filtro)
        {
            string query = QueryStringBuilder.ToQueryString(new { filtro = filtro });
            return await _httpClientService.GetAsync<List<PacienteDto>>($"Paciente/obtenerTodos/{query}");
        }

        public async Task<ServiceResponse> Agregar(PacienteDto paciente)
        {
            return await _httpClientService.PostAsync<PacienteDto, ServiceResponse>("Paciente/agregar", paciente);
        }

        public async Task<ServiceResponse> Modificar(PacienteDto paciente)
        {
            return await _httpClientService.PutAsync<PacienteDto, ServiceResponse>("Paciente/modificar", paciente);
        }

        public async Task<ServiceResponse> Eliminar(int id)
        {
            return await _httpClientService.DeleteAsync<ServiceResponse>($"Paciente/eliminar/{id}");
        }

        public List<string> ObtenerObrasSociales()
        {
            return Enum.GetNames(typeof(ObraSocialDto)).ToList();
        }
    }

    public interface IPacienteServiceWeb
    {
        Task<PacienteDto> ObtenerPorId(int id);
        Task<List<PacienteDto>> ObtenerTodos(string? filtro = null);
        Task<ServiceResponse> Agregar(PacienteDto medico);
        Task<ServiceResponse> Modificar(PacienteDto medico);
        Task<ServiceResponse> Eliminar(int id);
        List<string> ObtenerObrasSociales();
    }
}
