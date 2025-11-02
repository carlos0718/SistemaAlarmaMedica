using Dominio.Entidades;
using Dominio.Servicios.Medicos;
using Infraestructura.ContextoBD;
using Infraestructura.Genericos;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositorios
{
    public class MedicoRepository : Repository<Medico>, IMedicoRepository
    {
        private readonly IAplicacionBDContexto _dbContext;

        public MedicoRepository(IAplicacionBDContexto dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Especialidad>> ObtenerEspecialidadesAsync()
        {
            return await _dbContext.Set<Especialidad>().ToListAsync();
        }
    }
}
