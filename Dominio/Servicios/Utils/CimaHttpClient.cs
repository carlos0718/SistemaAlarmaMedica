using Dominio.Application.DTOs;
using Newtonsoft.Json;

namespace Dominio.Servicios.Utils
{
    public class CimaHttpClient : ICimaHttpClient
    {
        private readonly HttpClient _httpClient;
        private const string urlCima = "https://cima.aemps.es/cima/rest/medicamentos?";

        public CimaHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<FarmacoDto>> ObtenerMedicamentosPorNumeroRegistro(string numeroRegistro)
        {
            var response = await _httpClient.GetAsync($"{urlCima}nregistro={numeroRegistro}");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<ApiResponse<List<FarmacoDto>>>(json);

            return result.Resultados;
        }

        public async Task<List<FarmacoDto>> BuscarMedicamentosPorNombre(string nombre)
        {
            var response = await _httpClient.GetAsync($"{urlCima}nombre={nombre}");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<ApiResponse<List<FarmacoDto>>>(json);

            return result.Resultados;
        }

        public async Task<List<FarmacoDto>> ObtenerMedicamentosPorPagina(int numeroPagina)
        {
            var response = await _httpClient.GetAsync($"{urlCima}pagina={numeroPagina}");
            
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<ApiResponse<List<FarmacoDto>>>(json);

            return result.Resultados;
        }

     
    }
}
