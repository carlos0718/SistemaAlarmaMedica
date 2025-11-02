using Presentacion.Core.DTOs;
using Presentacion.Core.Responses;
using Presentacion.Tools.QueryBuilder;

namespace Presentacion.Services
{
    public class UsuarioServiceWeb : IUsuarioServiceWeb
    {
        private readonly HttpClientService _httpClientService;

        public UsuarioServiceWeb(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        #region USUARIO

        public async Task<UsuarioDto> ObtenerPorId(int id)
        {
            return await _httpClientService.GetAsync<UsuarioDto>($"Usuario/obtenerPorId/{id}");
        }

        public async Task<List<UsuarioDto>> ObtenerTodos(string? nombre)
        {
            string query = QueryStringBuilder.ToQueryString(new { nombre = nombre });
            return await _httpClientService.GetAsync<List<UsuarioDto>>($"Usuario/obtenerTodos{query}");
        }

        public async Task<ServiceResponse<UsuarioDto>> Agregar(UsuarioDto medico)
        {
            return await _httpClientService.PostAsync<UsuarioDto, ServiceResponse<UsuarioDto>>("Usuario/agregar", medico);
        }

        public async Task<ServiceResponse> Modificar(UsuarioDto medico)
        {
            return await _httpClientService.PutAsync<UsuarioDto, ServiceResponse>("Usuario/modificar", medico);
        }

        public List<string> ObtenerTiposUsuario()
        {
            return Enum.GetNames(typeof(TipoUsuarioDto)).ToList();
        }

        #endregion USUARIO

        #region LOGIN

        public async Task<ServiceResponse<UsuarioDto>> ObtenerPorNombreYContrasena(string nombre, string contrasena)
        {
            return await _httpClientService.GetAsync<ServiceResponse<UsuarioDto>>($"Usuario/obtenerPorNombreYContrasena?nombre={nombre}&contrasena={contrasena}");
        }

        public async Task<ServiceResponse<UsuarioDto>> ObtenerPorGoogleId(string googleId)
        {
            return await _httpClientService.GetAsync<ServiceResponse<UsuarioDto>>($"Usuario/obtenerPorGoogleId?googleId={googleId}");
        }

        public async Task<ServiceResponse<UsuarioDto>> ObtenerPorNombre(string nombre)
        {
            return await _httpClientService.GetAsync<ServiceResponse<UsuarioDto>>($"Usuario/obtenerPorNombre?nombre={nombre}");
        }

        public async Task<ServiceResponse> Eliminar(int id)
        {
            return await _httpClientService.DeleteAsync<ServiceResponse>($"Usuario/eliminar/{id}");
        }

        #endregion LOGIN
    }

    public interface IUsuarioServiceWeb
    {
        Task<UsuarioDto> ObtenerPorId(int id);
        Task<List<UsuarioDto>> ObtenerTodos(string? nombre = null);
        Task<ServiceResponse<UsuarioDto>> Agregar(UsuarioDto medico);
        Task<ServiceResponse> Modificar(UsuarioDto medico);
        List<string> ObtenerTiposUsuario();
        Task<ServiceResponse<UsuarioDto>> ObtenerPorNombreYContrasena(string nombre, string contrasena);
        Task<ServiceResponse<UsuarioDto>> ObtenerPorGoogleId(string googleId);
        Task<ServiceResponse<UsuarioDto>> ObtenerPorNombre(string nombre);
        Task<ServiceResponse> Eliminar(int id);
    }
}
