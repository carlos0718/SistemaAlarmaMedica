using Dominio.Core.Genericos;
using Dominio.Entidades;

namespace Dominio.Servicios.OrdenesMedicas
{
    public interface IOrdenMedicaRepository : IRepository<OrdenMedica>
    {
        Task<bool> ExistePacienteEnOrdenesMedicasAsync(int pacienteId);
        Task<bool> ExisteMedicoEnOrdenesMedicasAsync(int medicoId);
        Task<bool> ActualizarLineaOrdenMedicaAsync(int lineaOrdenMedicaId);
    }
}
