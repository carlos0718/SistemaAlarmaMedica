using Dominio.Entidades;
using Dominio.Servicios.Pacientes;
using Infraestructura.ContextoBD;
using Infraestructura.Genericos;

namespace Infraestructura.Repositorios
{
    public class PacienteRepository : Repository<Paciente>, IPacienteRepository
    {
        private readonly IAplicacionBDContexto _dbContext;

        public PacienteRepository(IAplicacionBDContexto dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
