using Dominio.Application.DTOs;
using Dominio.Servicios.Usuarios;
using Microsoft.AspNetCore.Mvc;
using PresentacionApi.Tools.FiltersDto;

namespace PresentacionApi.Controllers
{
    [Route("Usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        #region USUARIO

        [HttpGet("obtenerPorId/{id}", Name = "BuscarUsuarioPorId")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _usuarioService.ObtenerPorIdAsync(id);
            return Ok(result);
        }

        [HttpGet("obtenerTodos/", Name = "ObtenerTodosLosUsuarios")]
        public async Task<IActionResult> ObtenerTodos([FromQuery] FiltroUsuarioDto filtro)
        {
            var result = await _usuarioService.ObtenerTodosAsync(filtro.Nombre);
            return Ok(result);
        }

        [HttpPost("agregar/", Name = "AgregarUsuario")]
        public async Task<IActionResult> Agregar(UsuarioDto usuario)
        {
            var response = await _usuarioService.AgregarAsync(usuario);
            return Ok(response);
        }

        [HttpPut("modificar/", Name = "ModificarUsuario")]
        public async Task<IActionResult> Modificar(UsuarioDto usuario)
        {
            var response = await _usuarioService.ModificarAsync(usuario);
            return Ok(response);
        }

        [HttpDelete("eliminar/{id}", Name = "EliminarUsuario")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var response = await _usuarioService.EliminarAsync(id);
            return Ok(response);
        }

        #endregion USUARIO

        #region LOGIN

        [HttpGet("obtenerPorNombreYContrasena", Name = "ObtenerPorNombreYContrasena")]
        public async Task<IActionResult> ObtenerPorNombreYContrasena(string nombre, string contrasena)
        {
            var response = await _usuarioService.ObtenerPorNombreYContrasena(nombre, contrasena);
            return Ok(response);
        }

        [HttpGet("obtenerPorGoogleId", Name = "ObtenerPorGoogleId")]
        public async Task<IActionResult> ObtenerPorGoogleId(string googleId)
        {
            var response = await _usuarioService.ObtenerPorGoogleId(googleId);
            return Ok(response);
        }

        [HttpGet("obtenerPorNombre", Name = "ObtenerPorNombre")]
        public async Task<IActionResult> ObtenerPorNombre(string nombre)
        {
            var response = await _usuarioService.ObtenerPorNombre(nombre);
            return Ok(response);
        }

        #endregion LOGIN
    }
}
