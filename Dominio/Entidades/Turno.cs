namespace Dominio.Entidades
{
    public class Turno
    {
        public int TurnoId { get; set; }

        public int PacienteId { get; set; }
        public Paciente? Paciente { get; set; }

        public int MedicoId { get; set; }
        public Medico? Medico { get; set; }

        public DateTime FechaTurno { get; set; }

        public EstadoTurno Estado { get; set; }
    }
}
