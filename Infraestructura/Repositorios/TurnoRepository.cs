using Dominio.Entidades;
using Dominio.Servicios.Turnos;
using Infraestructura.ContextoBD;
using Infraestructura.Genericos;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositorios
{
    public class TurnoRepository : Repository<Turno>, ITurnoRepository
    {
        private readonly AplicacionBDContexto _context;

        public TurnoRepository(AplicacionBDContexto context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Turno>> ObtenerTurnosPorPacienteAsync(int pacienteId)
        {
            return await _context.Turnos
                .Where(t => t.PacienteId == pacienteId)
                .Include(t => t.Paciente)
                .Include(t => t.Medico)
                .ThenInclude(m => m.Especialidad)
                .ToListAsync();
        }

        public async Task<List<Turno>> ObtenerTurnosPorMedicoAsync(int medicoId)
        {
            return await _context.Turnos
                .Where(t => t.MedicoId == medicoId)
                .Include(t => t.Paciente)
                .Include(t => t.Medico)
                .ThenInclude(m => m.Especialidad)
                .ToListAsync();
        }
    }
}
