using Dominio.Entidades;
using Dominio.Servicios.OrdenesMedicas;
using Infraestructura.ContextoBD;
using Infraestructura.Genericos;

namespace Infraestructura.Repositorios
{
    public class LineaOrdenMedicaRepository : Repository<LineaOrdenMedica>, ILineaOrdenMedicaRepository
    {
        private readonly IAplicacionBDContexto _dbContext;

        public LineaOrdenMedicaRepository(IAplicacionBDContexto dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
