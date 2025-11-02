using Dominio.Entidades;
using Dominio.Servicios.Usuarios;
using Infraestructura.ContextoBD;
using Infraestructura.Genericos;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositorios
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        private readonly IAplicacionBDContexto _dbContext;

        public UsuarioRepository(IAplicacionBDContexto dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Usuario> ObtenerUsuarioPorNombre(string nombre)
        {
            return await _dbContext.Set<Usuario>().FirstOrDefaultAsync(usu => usu.Nombre == nombre);
        }

        public async Task<Usuario> ObtenerUsuarioPorGoogleId(string googleId)
        {
            return await _dbContext.Set<Usuario>().FirstOrDefaultAsync(usu => usu.GoogleId == googleId);
        }
    }
}
