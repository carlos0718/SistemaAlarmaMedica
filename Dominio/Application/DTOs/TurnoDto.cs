using Dominio.Entidades;

namespace Dominio.Application.DTOs
{
    public class TurnoDto
    {
        public int? TurnoId { get; set; }

        public int? PacienteId { get; set; }
        public PacienteDto? Paciente { get; set; }

        public int? MedicoId { get; set; }
        public MedicoDto? Medico { get; set; }

        public DateTime? FechaTurno { get; set; }

        public EstadoTurno? Estado { get; set; }
    }
}
