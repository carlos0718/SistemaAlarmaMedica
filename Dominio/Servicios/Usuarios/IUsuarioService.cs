using Dominio.Application.DTOs;
using Dominio.Shared;

namespace Dominio.Servicios.Usuarios
{
    public interface IUsuarioService
    {
        Task<UsuarioDto> ObtenerPorIdAsync(int id);
        Task<List<UsuarioDto>> ObtenerTodosAsync(string? nombre);
        Task<ServiceResponse> AgregarAsync(UsuarioDto entity);
        Task<ServiceResponse> ModificarAsync(UsuarioDto entity);
        Task<ServiceResponse> EliminarAsync(int id);
        Task<ServiceResponse<UsuarioDto>> ObtenerPorNombreYContrasena(string nombre, string contrasena);
        Task<ServiceResponse<UsuarioDto>> ObtenerPorGoogleId(string googleId);
        Task<ServiceResponse<UsuarioDto>> ObtenerPorNombre(string nombre);
    }
}
