using Presentacion.Core.DTOs;
using Presentacion.Core.Responses;
using Presentacion.Tools.QueryBuilder;

namespace Presentacion.Services
{
    public class MedicoServiceWeb : IMedicoServiceWeb
    {
        private readonly HttpClientService _httpClientService;

        public MedicoServiceWeb(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<MedicoDto> ObtenerPorId(int id)
        {
            return await _httpClientService.GetAsync<MedicoDto>($"Medico/obtenerPorId/{id}");
        }

        public async Task<List<MedicoDto>> ObtenerTodos(string? filtro)
        {
            string query = QueryStringBuilder.ToQueryString(new { filtro = filtro });
            return await _httpClientService.GetAsync<List<MedicoDto>>($"Medico/obtenerTodos/{query}");
        }

        public async Task<ServiceResponse> Agregar(MedicoDto medico)
        {
            return await _httpClientService.PostAsync<MedicoDto, ServiceResponse>("Medico/agregar", medico);
        }

        public async Task<ServiceResponse> Modificar(MedicoDto medico)
        {
            return await _httpClientService.PutAsync<MedicoDto, ServiceResponse>("Medico/modificar", medico);
        }

        public async Task<ServiceResponse> Eliminar(int id)
        {
            return await _httpClientService.DeleteAsync<ServiceResponse>($"Medico/eliminar/{id}");
        }

        public async Task<List<EspecialidadDto>> ObtenerEspecialidades()
        {
            return await _httpClientService.GetAsync<List<EspecialidadDto>>($"Medico/obtenerEspecialidades/");
        }
    }

    public interface IMedicoServiceWeb
    {
        Task<MedicoDto> ObtenerPorId(int id);
        Task<List<MedicoDto>> ObtenerTodos(string? filtro = null);
        Task<ServiceResponse> Agregar(MedicoDto medico);
        Task<ServiceResponse> Modificar(MedicoDto medico);
        Task<ServiceResponse> Eliminar(int id);

        Task<List<EspecialidadDto>> ObtenerEspecialidades();
    }
}
