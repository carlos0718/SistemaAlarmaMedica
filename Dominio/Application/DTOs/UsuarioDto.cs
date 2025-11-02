namespace Dominio.Application.DTOs
{
    public class UsuarioDto
    {
        public int? UsuarioId { get; set; }
        public string? Nombre { get; set; }
        public string? Contrasena { get; set; }
        public TipoUsuarioDto? TipoUsuario { get; set; }
        public bool? Activo { get; set; }
        public string? GoogleId { get; set; }
        public string? Email { get; set; }
        public int? PacienteId { get; set; }
        public int? MedicoId { get; set; }
    }
}
