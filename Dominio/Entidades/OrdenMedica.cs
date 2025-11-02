namespace Dominio.Entidades
{
    public class OrdenMedica
    {
        public int OrdenMedicaId { get; set; }
        
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        
        public int MedicoId { get; set; }
        public Medico Medico { get; set; }
        
        public DateTime Fecha { get; set; }
        public ObraSocial? ObraSocial { get; set; }
        public bool EntregadaAlPaciente { get; set; }
        
        public List<LineaOrdenMedica> LineaOrdenMedica { get; set; }
    }
}
