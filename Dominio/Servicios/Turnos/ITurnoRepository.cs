using Dominio.Core.Genericos;
using Dominio.Entidades;

namespace Dominio.Servicios.Turnos
{
    public interface ITurnoRepository : IRepository<Turno>
    {
        Task<List<Turno>> ObtenerTurnosPorPacienteAsync(int pacienteId);
        Task<List<Turno>> ObtenerTurnosPorMedicoAsync(int medicoId);
    }
}
