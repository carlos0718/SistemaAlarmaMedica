namespace Presentacion.Core.DTOs
{
    public class TurnoDto
    {
        public int? TurnoId { get; set; }
        public int? PacienteId { get; set; }
        public PacienteDto? Paciente { get; set; }
        public int? MedicoId { get; set; }
        public MedicoDto? Medico { get; set; }
        public DateTime? FechaTurno { get; set; }
        public EstadoTurnoDto? Estado { get; set; }

        // Propiedad calculada: indica si el turno fue atendido (si existe una OrdenMedica)
        public bool FueAtendido { get; set; }
    }
}
