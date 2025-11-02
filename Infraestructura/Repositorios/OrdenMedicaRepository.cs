using Dominio.Entidades;
using Dominio.Servicios.OrdenesMedicas;
using Infraestructura.ContextoBD;
using Infraestructura.Genericos;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositorios
{
    public class OrdenMedicaRepository : Repository<OrdenMedica>, IOrdenMedicaRepository
    {
        private readonly IAplicacionBDContexto _dbContext;

        public OrdenMedicaRepository(IAplicacionBDContexto dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistePacienteEnOrdenesMedicasAsync(int pacienteId)
        {
            return await _dbContext.Set<OrdenMedica>().Where(om => om.PacienteId == pacienteId).AnyAsync();
        }

        public async Task<bool> ExisteMedicoEnOrdenesMedicasAsync(int medicoId)
        {
            return await _dbContext.Set<OrdenMedica>().Where(om => om.MedicoId == medicoId).AnyAsync();
        }

        public async Task<bool> ActualizarLineaOrdenMedicaAsync(int lineaOrdenMedicaId)
        {
            var lineaOrdenMedica = await _dbContext.Set<LineaOrdenMedica>()
                .FirstOrDefaultAsync(l => l.LineaOrdenMedicaId == lineaOrdenMedicaId);

            if (lineaOrdenMedica == null)
                return false;

            lineaOrdenMedica.TratamientoEmpezado = true;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
