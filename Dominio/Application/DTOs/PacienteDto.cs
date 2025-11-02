namespace Dominio.Application.DTOs
{
    public class PacienteDto
    {
        public int? PacienteId { get; set; }
        public int? Documento { get; set; }
        public string? Apellido { get; set; }
        public string? Nombre { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }
}
