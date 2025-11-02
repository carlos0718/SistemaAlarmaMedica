namespace Presentacion.Core.DTOs
{
    public class MedicoDto
    {
        public int? MedicoId { get; set; }
        public string? Apellido { get; set; }
        public string? Nombre { get; set; }
        public string? Matricula { get; set; }

        public int? EspecialidadId { get; set; }
        public EspecialidadDto? Especialidad { get; set; }

        public string NombreCompleto => $"{Apellido}, {Nombre} || Matric: {Matricula}";
    }
}
