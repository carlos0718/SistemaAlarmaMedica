using AutoMapper;
using Dominio.Application.DTOs;
using Dominio.Entidades;
using Dominio.Shared;

namespace Dominio.Servicios.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IMapper mapper, IUsuarioRepository usuarioRepository)
        {
            _mapper = mapper;
            _usuarioRepository = usuarioRepository;
        }

        #region USUARIO

        public async Task<UsuarioDto> ObtenerPorIdAsync(int id)
        {
            var usuarioDb = await _usuarioRepository.GetByIdAsync(id);
            return _mapper.Map<UsuarioDto>(usuarioDb);
        }

        public async Task<List<UsuarioDto>> ObtenerTodosAsync(string? nombre)
        {
            var usuariosDb = await _usuarioRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(nombre))
                usuariosDb = usuariosDb.Where(u => u.Nombre != null && u.Nombre.ToLower().Contains(nombre.ToLower())).ToList();

            return _mapper.Map<List<UsuarioDto>>(usuariosDb);
        }

        public async Task<ServiceResponse> AgregarAsync(UsuarioDto entity)
        {
            var response = new ServiceResponse();
            try
            {
                var validarNombre = (await _usuarioRepository.GetAllAsync()).Any(m => m.Nombre == entity.Nombre);
                if (validarNombre)
                    throw new InvalidOperationException($"El nombre del usuario {entity.Nombre} ya existe en la base de datos");

                var usuario = _mapper.Map<Usuario>(entity);
                await _usuarioRepository.AddAsync(usuario);
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }
            return response;
        }

        public async Task<ServiceResponse> ModificarAsync(UsuarioDto entity)
        {
            var response = new ServiceResponse();

            try
            {
                var usuario = _mapper.Map<Usuario>(entity);
                await _usuarioRepository.UpdateAsync(usuario);
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }
            return response;
        }

        public async Task<ServiceResponse> EliminarAsync(int id)
        {
            var response = new ServiceResponse();

            try
            {
                if ((await _usuarioRepository.GetByIdAsync(id)) == null)
                    throw new InvalidOperationException($"El Id del Usuario no exite en la base de datos");

                var usuarioDb = await _usuarioRepository.GetByIdAsync(id);
                await _usuarioRepository.DeleteAsync(usuarioDb);
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }
            return response;
        }

        #endregion USUARIO

        #region LOGIN

        public async Task<ServiceResponse<UsuarioDto>> ObtenerPorNombreYContrasena(string nombre, string contrasena)
        {
            var response = new ServiceResponse<UsuarioDto>();

            try
            {
                var usuarioDb = await _usuarioRepository.ObtenerUsuarioPorNombre(nombre);

                if (usuarioDb == null)
                    throw new ArgumentException($"El Usuario {nombre} No Existe.");

                if (!usuarioDb.Activo)
                    throw new ArgumentException($"Usuario {nombre} se encuentra Inactivo");

                if (usuarioDb.Contrasena != contrasena)
                    throw new ArgumentException("Contraseña incorrecta");

                response.Data = _mapper.Map<UsuarioDto>(usuarioDb);
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }

            return response;
        }

        public async Task<ServiceResponse<UsuarioDto>> ObtenerPorGoogleId(string googleId)
        {
            var response = new ServiceResponse<UsuarioDto>();

            try
            {
                var usuarioDb = await _usuarioRepository.ObtenerUsuarioPorGoogleId(googleId);

                if (usuarioDb != null && !usuarioDb.Activo)
                    throw new ArgumentException($"Usuario se encuentra Inactivo");

                response.Data = usuarioDb != null ? _mapper.Map<UsuarioDto>(usuarioDb) : null;
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }

            return response;
        }

        public async Task<ServiceResponse<UsuarioDto>> ObtenerPorNombre(string nombre)
        {
            var response = new ServiceResponse<UsuarioDto>();

            try
            {
                var usuarioDb = await _usuarioRepository.ObtenerUsuarioPorNombre(nombre);

                if (usuarioDb == null)
                    throw new ArgumentException($"El Usuario {nombre} No Existe.");

                if (!usuarioDb.Activo)
                    throw new ArgumentException($"Usuario {nombre} se encuentra Inactivo");

                response.Data = _mapper.Map<UsuarioDto>(usuarioDb);
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }

            return response;
        }

        #endregion LOGIN
    }
}
