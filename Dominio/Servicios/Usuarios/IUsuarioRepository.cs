using Dominio.Core.Genericos;
using Dominio.Entidades;

namespace Dominio.Servicios.Usuarios
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario> ObtenerUsuarioPorNombre(string nombre);
        Task<Usuario> ObtenerUsuarioPorGoogleId(string googleId);
    }
}
