using Presentacion.Core.DTOs;
using Presentacion.Core.Responses;
using Presentacion.Tools.QueryBuilder;

namespace Presentacion.Services
{
    public class OrdenMedicaServiceWeb : IOrdenMedicaServiceWeb
    {
        private readonly HttpClientService _httpClientService;

        public OrdenMedicaServiceWeb(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<OrdenMedicaDto> ObtenerPorId(int id)
        {
            return await _httpClientService.GetAsync<OrdenMedicaDto>($"OrdenMedica/obtenerPorId/{id}");
        }

        public async Task<List<OrdenMedicaDto>> ObtenerTodos(string? filtro)
        {
            string query = QueryStringBuilder.ToQueryString(new { filtro = filtro });
            return await _httpClientService.GetAsync<List<OrdenMedicaDto>>($"OrdenMedica/obtenerTodos{query}");
        }

        public async Task<ServiceResponse> Agregar(OrdenMedicaDto ordenMedica)
        {
            return await _httpClientService.PostAsync<OrdenMedicaDto, ServiceResponse>("OrdenMedica/agregar", ordenMedica);
        }

        public async Task<ServiceResponse> Modificar(OrdenMedicaDto ordenMedica)
        {
            return await _httpClientService.PutAsync<OrdenMedicaDto, ServiceResponse>("OrdenMedica/modificar", ordenMedica);
        }

        public async Task<ServiceResponse> Eliminar(int id)
        {
            return await _httpClientService.DeleteAsync<ServiceResponse>($"OrdenMedica/eliminar/{id}");
        }
    }

    public interface IOrdenMedicaServiceWeb
    {
        Task<OrdenMedicaDto> ObtenerPorId(int id);
        Task<List<OrdenMedicaDto>> ObtenerTodos(string? filtro = null);
        Task<ServiceResponse> Agregar(OrdenMedicaDto medico);
        Task<ServiceResponse> Modificar(OrdenMedicaDto medico);
        Task<ServiceResponse> Eliminar(int id);
    }
}
