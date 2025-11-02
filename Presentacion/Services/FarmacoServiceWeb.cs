using Presentacion.Core.DTOs;

namespace Presentacion.Services
{
    public class FarmacoServiceWeb : IFarmacoServiceWeb
    {
        private readonly HttpClientService _httpClientService;

        public FarmacoServiceWeb(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<FarmacoDto> ObtenerFarmacosPorNumeroRegistro(string numeroRegistro)
        {
            var response = await _httpClientService.GetAsync<FarmacoDto>($"Farmaco/buscarPorNumeroRegistro/{numeroRegistro}");
            return response;
        }

        public async Task<List<FarmacoDto>> ObtenerFarmacosPorNombre(string nombre)
        {
            return await _httpClientService.GetAsync<List<FarmacoDto>>($"Farmaco/buscarPorNombre/{nombre}");
        }

        public async Task<List<FarmacoDto>> ObtenerFarmacosPorPagina(int pagina)
        {
            return await _httpClientService.GetAsync<List<FarmacoDto>>($"Farmaco/buscarPorPagina/{pagina}");
        }
    }

    public interface IFarmacoServiceWeb
    {
        Task<FarmacoDto> ObtenerFarmacosPorNumeroRegistro(string numeroRegistro);
        Task<List<FarmacoDto>> ObtenerFarmacosPorNombre(string nombre);
        Task<List<FarmacoDto>> ObtenerFarmacosPorPagina(int pagina);
    }
}
